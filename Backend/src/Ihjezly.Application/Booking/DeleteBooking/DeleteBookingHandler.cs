using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Booking.Repositories;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Booking.DeleteBooking;

internal sealed class DeleteBookingHandler : ICommandHandler<DeleteBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBookingHandler(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result.Failure(BookingErrors.NotFound);

        var property = await _propertyRepository.GetByIdNonGeniricAsync(booking.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure(PropertyErrors.NotFound);

        // Business owner ID from the property
        var businessOwnerId = property.BusinessOwnerId;

        // Notification for business owner
        var businessOwnerMessage = "تم حذف الحجز.";
        var businessOwnerNotification = Notification.Create(businessOwnerId, businessOwnerMessage);
        _notificationRepository.Add(businessOwnerNotification);

        // Notification for client
        var clientMessage = "تم حذف حجزك.";
        var clientNotification = Notification.Create(booking.ClientId, clientMessage);
        _notificationRepository.Add(clientNotification);

        _bookingRepository.Remove(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}