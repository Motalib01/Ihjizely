using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Reviews;

namespace Ihjezly.Application.Reviews.CreateReview;

internal sealed class CreateReviewHandler : ICommandHandler<CreateReviewCommand, Guid>
{
    private readonly IReviewRepository _repository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewHandler(
        IReviewRepository repository,
        IPropertyRepository propertyRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = Review.Create(request.PropertyId, request.UserId, request.Rating, request.Comment);
        _repository.Add(review);

        var property = await _propertyRepository.GetByIdNonGeniricAsync(request.PropertyId, cancellationToken);
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
