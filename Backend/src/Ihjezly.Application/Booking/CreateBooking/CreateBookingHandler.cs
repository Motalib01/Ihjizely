using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Booking.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking.Repositories;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Shared;
using BookingEntity = Ihjezly.Domain.Booking.Booking;

namespace Ihjezly.Application.Booking.CreateBooking;

internal sealed class CreateBookingHandler : ICommandHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _repository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingHandler(
        IBookingRepository repository,
        IPropertyRepository propertyRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1) Get property
        var property = await _propertyRepository.GetByIdNonGeniricAsync(request.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure<BookingDto>(PropertyErrors.NotFound);

        // 2) Check if client already has overlapping booking
        var clientBookings = await _repository.GetByClientIdAsync(request.ClientId, cancellationToken);

        bool hasOverlap = clientBookings.Any(b =>
            b.Status != Domain.Booking.BookingStatus.Rejected && // ignore rejected bookings
            b.StartDate < request.EndDate &&
            request.StartDate < b.EndDate
        );

        if (hasOverlap)
        {
            return Result.Failure<BookingDto>(
                new Error("Booking.Overlap", "لا يمكنك إجراء أكثر من حجز لنفس الفترة الزمنية.")
            );
        }

        // 3) Create booking
        var booking = BookingEntity.Reserve(
            request.ClientId,
            request.Name,
            request.PhoneNumber,
            request.PropertyId,
            property,
            request.StartDate,
            request.EndDate
        );

        _repository.Add(booking);

        // 4) Notify business owner
        var businessOwnerId = property.BusinessOwnerId;
        var ownerNotification = Notification.Create(
            businessOwnerId,
            $"لديك طلب حجز جديد لعقارك: {property.Title}"
        );
        _notificationRepository.Add(ownerNotification);

        // 5) Save all
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6) Map to DTO
        var dto = new BookingDto(
            booking.Id,
            booking.ClientId,
            booking.Name,
            booking.PhoneNumber,
            booking.PropertyId,
            booking.StartDate,
            booking.EndDate,
            booking.TotalPrice.Amount,
            booking.TotalPrice.Currency.Code,
            booking.Status.ToString(),
            booking.ReservedAt
        );

        return dto;
    }
}
