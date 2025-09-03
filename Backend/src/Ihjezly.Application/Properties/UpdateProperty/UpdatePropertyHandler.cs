using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Application.Properties.UpdateProperty;

internal sealed class UpdatePropertyHandler<TProperty, TDetails>
    : ICommandHandler<UpdatePropertyCommand<TProperty, TDetails>>
    where TProperty : PropertyWithDetails<TDetails>, new()
{
    private readonly IPropertyRepository<TProperty> _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePropertyHandler(
        IPropertyRepository<TProperty> repository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdatePropertyCommand<TProperty, TDetails> request, CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure(PropertyErrors.NotFound);

        var location = request.Location.ToDomain();
        var money = new Money(request.Price, Currency.FromCode(request.Currency));
        var discount = request.Discount?.ToDomain();

        property.Update(
            request.Title,
            request.Description,
            location,
            money,
            request.Details,
            discount,
            request.VideoUrl,
            request.Unavailables,
            request.Facilities
        );

        property.ChangeType(request.Type);

        if (request.DeletedImages is not null)
        {
            foreach (var imageUrl in request.DeletedImages)
            {
                property.RemoveImage(imageUrl);
            }
        }

        if (request.NewImages is not null)
        {
            foreach (var imageUrl in request.NewImages)
            {
                try
                {
                    var image = Image.Create(imageUrl);
                    property.AddImage(image);
                }
                catch
                {
                    return Result.Failure(PropertyErrors.InvalidImage);
                }
            }
        }

        var userIdToNotify = property.BusinessOwnerId;
        var notificationMessage = $"تم تحديث عقارك '{property.Title}'.";

        var notification = Notification.Create(userIdToNotify, notificationMessage);
        _notificationRepository.Add(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
