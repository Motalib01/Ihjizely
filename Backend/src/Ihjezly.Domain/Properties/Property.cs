using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Properties;

public abstract class Property : Entity
{
    public readonly List<Image> _images = new();
    public IReadOnlyCollection<Image> Images => _images.AsReadOnly();

    protected List<Facility> _facilities = new();
    public IReadOnlyCollection<Facility> Facilities => _facilities.AsReadOnly();


    protected Property(Guid id ) : base(id) { }

    public string Title { get; protected set; } = string.Empty;
    public string Description { get; protected set; } = string.Empty;
    public Location Location { get; protected set; } = default!;
    public Money Price { get; protected set; } = default!;
    public Discount? Discount { get; protected set; }
    public PropertyType Type { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public Guid BusinessOwnerId { get; protected set; }
    public bool IsAd { get; protected set; }
    public PropertyStatus Status { get; protected set; } = PropertyStatus.Pending;
    public ViedeoUrl? ViedeoUrl { get; protected set; } 
    public List<DateTime>? Unavailbles { get; protected set; } 

    protected Property(
        string title,
        string description,
        Location location,
        Money price,
        PropertyType type,
        Guid businessOwnerId,
        ViedeoUrl? viedeoUrl,
        bool isAd,
        List<DateTime>? unavailbles= null,
        Discount? discount = null,
        List<Facility>? facilities = null
         )
        : this(Guid.NewGuid())
    {
        Title = title;
        Description = description;
        Location = location;
        Price = price;
        Type = type;
        BusinessOwnerId = businessOwnerId;
        IsAd = isAd;
        Discount = discount;
        ViedeoUrl = viedeoUrl;


        if (facilities is not null)
            _facilities.AddRange(facilities);

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Unavailbles = unavailbles ?? new List<DateTime>();
    }

    
    public void ChangeStatus(PropertyStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    // ==== Image Methods ====

    public void AddImage(Image image)
    {
        if (_images.Any(img => img.Url == image.Url))
            throw new InvalidOperationException("Image already exists.");

        _images.Add(image);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveImage(string imageUrl)
    {
        var image = _images.FirstOrDefault(img => img.Url == imageUrl);
        if (image is null)
            throw new InvalidOperationException("Image not found.");

        _images.Remove(image);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearImages()
    {
        _images.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetImages(IEnumerable<Image> images)
    {
        _images.Clear();
        _images.AddRange(images);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeType(PropertyType newType)
    {
        Type = newType;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetIsAd(bool isAd)
    {
        IsAd = isAd;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetFacilities(IEnumerable<Facility> facilities)
    {
        _facilities = facilities?.ToList() ?? new List<Facility>();
        UpdatedAt = DateTime.UtcNow;
    }


    public void AddFacility(Facility facility)
    {
        if (!_facilities.Contains(facility))
        {
            _facilities.Add(facility);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveFacility(Facility facility)
    {
        if (_facilities.Contains(facility))
        {
            _facilities.Remove(facility);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ClearFacilities()
    {
        _facilities.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

}