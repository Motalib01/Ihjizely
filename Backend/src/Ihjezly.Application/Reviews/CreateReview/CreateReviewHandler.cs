using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Booking.Repositories;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Reviews;

namespace Ihjezly.Application.Reviews.CreateReview;

internal sealed class CreateReviewHandler : ICommandHandler<CreateReviewCommand, Guid>
{
    private readonly IReviewRepository _repository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewHandler(
        IReviewRepository repository,
        IPropertyRepository propertyRepository,
        INotificationRepository notificationRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // 1. Ensure booking exists and belongs to the client
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result.Failure<Guid>(ReviewErrors.NotFound);

        if (booking.ClientId != request.UserId)
            return Result.Failure<Guid>(ReviewErrors.CantReview);

        // 2. Ensure booking is completed
        if (booking.Status != BookingStatus.Completed)
            return Result.Failure<Guid>(ReviewErrors.CantReview);

        // 3. Prevent duplicate reviews for the same booking
        var existingReview = await _repository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (existingReview is not null)
            return Result.Failure<Guid>(ReviewErrors.CantReview);

        // 4. Create review
        var review = Review.Create(booking.PropertyId, request.UserId, booking.Id, request.Rating, request.Comment);
        _repository.Add(review);

        // 5. Notify client + owner
        var property = await _propertyRepository.GetByIdNonGeniricAsync(booking.PropertyId, cancellationToken);
        if (property is not null)
        {
            var userNotification = Notification.Create(
                request.UserId,
                "تم إرسال تقييمك بنجاح."
            );
            _notificationRepository.Add(userNotification);

            var ownerNotification = Notification.Create(
                property.BusinessOwnerId,
                $"لقد تلقيت تقييما جديدا لعقارك '{property.Title}'."
            );
            _notificationRepository.Add(ownerNotification);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return review.Id;
    }

}
