using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

public sealed record UpdateHallCommand<TProperty, TDetails>(
    Guid Id,
    string Title,
    string Description,
    Money Price,
    TDetails? Details,
    Discount? Discount,
    ViedeoUrl? VideoUrl,
    List<string>? ImagesToAdd = null,
    List<string>? ImagesToRemove = null,
    List<DateTime>? Unavailbles = null
) : IRequest<Result<Guid>>
    where TProperty : HallProperty<TDetails>;

internal sealed class UpdateHallCommandHandler<TProperty, TDetails>
    : IRequestHandler<UpdateHallCommand<TProperty, TDetails>, Result<Guid>>
    where TProperty : HallProperty<TDetails>
{
    private readonly IPropertyRepository<TProperty> _propertyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHallCommandHandler(
        IPropertyRepository<TProperty> propertyRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateHallCommand<TProperty, TDetails> request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (property is null)
            return Result.Failure<Guid>(PropertyErrors.NotFound);

        
            // Update property details
            property.Update(
                title: request.Title,
                description: request.Description,
                location: property.Location,
                price: request.Price,
                details: request.Details,
                discount: request.Discount,
                viedeoUrl: request.VideoUrl,
                unavailbles: request.Unavailbles
            );

            // Remove images
            if (request.ImagesToRemove != null)
                foreach (var url in request.ImagesToRemove)
                    property.RemoveImage(url);

            // Add images
            if (request.ImagesToAdd != null)
                foreach (var url in request.ImagesToAdd)
                    property.AddImage(new Image(url));

            // Optional: create a notification
            var notification = Notification.Create(
                property.BusinessOwnerId,
                $"تم تحديث قاعة عقارك '{property.Title}' بنجاح."
            );
            _notificationRepository.Add(notification);

            // Persist all changes via UnitOfWork
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(property.Id);
        
        
    }
}

// Request DTO
public class UpdateHallPropertyRequest<TProperty, TDetails>
    where TProperty : HallProperty<TDetails>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal PriceAmount { get; set; }
    public string PriceCurrencyCode { get; set; } = "LYD";
    public TDetails? Details { get; set; } = default!;
    public Discount? Discount { get; set; }
    public string? VideoUrl { get; set; }
    public List<DateTime>? Unavailbles { get; set; }
    public List<IFormFile>? Images { get; set; }
    public List<string>? ImagesToRemove { get; set; }

    public UpdateHallCommand<TProperty, TDetails> ToCommand(Guid id, List<string>? uploadedImageUrls = null)
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
            Unavailbles: Unavailbles
        );
}
