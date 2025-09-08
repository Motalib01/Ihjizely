using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Users.Repositories;

internal sealed class CreatePropertyHandler<TProperty, TDetails>
    : ICommandHandler<CreatePropertyCommand<TProperty, TDetails>, Guid>
    where TProperty : PropertyWithDetails<TDetails>, new()
{
    private readonly IPropertyRepository<TProperty> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public CreatePropertyHandler(
        IPropertyRepository<TProperty> repository,
        IUnitOfWork unitOfWork,
        INotificationRepository notificationRepository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> Handle(CreatePropertyCommand<TProperty, TDetails> request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return Result.Failure<Guid>(PropertyErrors.TitleRequired);

        if (!Currency.IsValidCode(request.Currency))
            return Result.Failure<Guid>(PropertyErrors.InvalidCurrency);

        var location = new Location(
            request.Location.City,
            request.Location.State,
            request.Location.Country,
            request.Location.Latitude,
            request.Location.Longitude);

        var currency = Currency.FromCode(request.Currency);
        var price = new Money(request.Price, currency);
        var discount = request.Discount?.ToDomain();

        List<Facility>? facilities = null;
        var tempPropertyInstance = new TProperty();
        if (tempPropertyInstance.SupportsFacilities)
        {
            facilities = request.Facilities ?? new List<Facility>();
        }

        var property = PropertyWithDetails<TDetails>.Create<TProperty>(
            request.Title,
            request.Description,
            location,
            price,
            request.Type,
            request.Details,
            request.ViedeoUrl,
            request.BusinessOwnerId,
            request.IsAd,
            request.Unavailables ?? new List<DateTime>(),
            discount,
            facilities);

        if (request.Images is not null)
        {
            foreach (var image in request.Images)
            {
                try
                {
                    property.AddImage(image);
                }
                catch (ArgumentException)
                {
                    return Result.Failure<Guid>(PropertyErrors.InvalidImage);
                }
            }
        }


        _repository.Add(property);

        var admins = await _userRepository.GetAdminsAsync(cancellationToken);
        var message = $"تم إرسال العقار الجديد '{property.Title}' من قبل صاحب العمل {property.BusinessOwnerId}";

        foreach (var admin in admins)
        {
            var notification = Notification.Create(admin.Id, message);
            _notificationRepository.Add(notification);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return property.Id;
    }
}
