using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ihjezly.Application.Properties.Residence.UpdateResidence;

// Command
public sealed record UpdateResidenceCommand<TProperty, TDetails>(
    Guid Id,
    string Title,
    string Description,
    Money Price,
    TDetails Details,
    Discount? Discount,
    ViedeoUrl? VideoUrl,
    List<string>? ImagesToAdd = null,
    List<string>? ImagesToRemove = null,
    List<DateTime>? Unavailables = null,
    List<Facility>? Facilities = null
) : IRequest<Result<Guid>>
    where TProperty : ResidenceProperty<TDetails>;

// Handler
internal sealed class UpdateResidenceCommandHandler<TProperty, TDetails>
    : IRequestHandler<UpdateResidenceCommand<TProperty, TDetails>, Result<Guid>>
    where TProperty : ResidenceProperty<TDetails>
{
    private readonly IPropertyRepository<TProperty> _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRepository _notificationRepository;

    public UpdateResidenceCommandHandler(
        IPropertyRepository<TProperty> propertyRepository,
        IUnitOfWork unitOfWork,
        INotificationRepository notificationRepository)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<Guid>> Handle(UpdateResidenceCommand<TProperty, TDetails> request,
        CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (property is null)
            return Result.Failure<Guid>(PropertyErrors.NotFound);

        // Update core details (facilities handled separately)
        property.Update(
            title: request.Title,
            description: request.Description,
            location: property.Location,
            price: request.Price,
            details: request.Details,
            discount: request.Discount,
            viedeoUrl: request.VideoUrl,
            unavailbles: request.Unavailables,
            facilities: null // keep null to avoid override
        );

        // === Facilities update ===
        if (request.Facilities is not null)
        {
            property.SetFacilities(request.Facilities);
        }

        // === Images update ===
        if (request.ImagesToRemove is not null)
        {
            foreach (var url in request.ImagesToRemove)
                property.RemoveImage(url);
        }

        if (request.ImagesToAdd is not null)
        {
            foreach (var url in request.ImagesToAdd)
                property.AddImage(new Image(url));
        }

        // === Notification ===
        var notification = Notification.Create(
            property.BusinessOwnerId,
            $"تم تحديث عقارك '{property.Title}' بنجاح."
        );
        _notificationRepository.Add(notification);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(property.Id);
    }
}

// Request DTO
public class UpdateResidencePropertyRequest<TProperty, TDetails>
    where TProperty : ResidenceProperty<TDetails>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal PriceAmount { get; set; }
    public string PriceCurrencyCode { get; set; } = "LYD";
    public TDetails Details { get; set; } = default!;
    public Discount? Discount { get; set; }
    public string? VideoUrl { get; set; }
    public List<DateTime>? Unavailables { get; set; }
    public List<Facility>? Facilities { get; set; }
    public List<IFormFile>? Images { get; set; }
    public List<string>? ImagesToRemove { get; set; }

    public UpdateResidenceCommand<TProperty, TDetails> ToCommand(Guid id, List<string>? uploadedImageUrls = null)
        => new(
            Id: id,
            Title: Title,
            Description: Description,
            Price: new Money(PriceAmount, Currency.FromCode(PriceCurrencyCode)),
            Details: Details,
            Discount: Discount,
            VideoUrl: string.IsNullOrEmpty(VideoUrl) ? null : new ViedeoUrl(VideoUrl),
            ImagesToAdd: uploadedImageUrls,
            ImagesToRemove: ImagesToRemove,
            Unavailables: Unavailables,
            Facilities: Facilities
        );
}
