using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public abstract class PropertyWithDetails<TDetails> : Property
{
    public TDetails Details { get; protected set; } = default!;

    // Allow child classes to override this flag to control facility support
    public virtual bool SupportsFacilities => true;

    protected PropertyWithDetails(Guid id) : base(id) { }

    protected PropertyWithDetails(
        string title,
        string description,
        Location location,
        Money price,
        PropertyType type,
        Guid businessOwnerId,
        bool isAd,
        TDetails details,
        ViedeoUrl viedeoUrl,
        List<DateTime>? unavailbles = null,
        Discount? discount = null,
        List<Facility>? facilities = null)
        : base(title, description, location, price, type, businessOwnerId, viedeoUrl, isAd, unavailbles,  discount, facilities)
    {
        Details = details;
        RaiseDomainEvent(new PropertyCreatedDomainEvent(Id));
    }

    public static TProperty Create<TProperty>(
        string title,
        string description,
        Location location,
        Money price,
        PropertyType type,
        TDetails details,
        ViedeoUrl viedeoUrl,
        Guid businessOwnerId,
        bool isAd,
        List<DateTime>? unavailbles = null,
        Discount? discount = null,
        List<Facility>? facilities = null 
         )
        where TProperty : PropertyWithDetails<TDetails>, new()
    {
        var property = new TProperty
        {
            Title = title,
            Description = description,
            Location = location,
            Price = price,
            Type = type,
            Details = details,
            ViedeoUrl = viedeoUrl,
            Unavailbles = unavailbles,
            Discount = discount,
            BusinessOwnerId = businessOwnerId,
            IsAd = isAd,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            
        };


        if (facilities is not null)
        {
            if (!property.SupportsFacilities)
                throw new InvalidOperationException($"The property type '{typeof(TProperty).Name}' does not support facilities.");

            property._facilities.AddRange(facilities);
        }

        property.RaiseDomainEvent(new PropertyCreatedDomainEvent(property.Id));
        return property;
    }

    public void Update(
        string title,
        string description,
        Location location,
        Money price,
        TDetails details,
        Discount? discount,
        ViedeoUrl viedeoUrl,
        List<DateTime>? unavailbles = null,
        List<Facility>? facilities = null)
    {
        Title = title;
        Description = description;
        Location = location;
        Price = price;
        Discount = discount;
        Details = details;
        ViedeoUrl = viedeoUrl;
        Unavailbles = unavailbles;

        if (facilities is not null)
        {
            if (!SupportsFacilities)
                throw new InvalidOperationException($"The property type '{GetType().Name}' does not support facilities.");

            _facilities.Clear();
            _facilities.AddRange(facilities);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        RaiseDomainEvent(new PropertyDeletedDomainEvent(Id));
    }

   

}