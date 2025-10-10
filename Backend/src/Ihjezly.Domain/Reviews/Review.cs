using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Reviews.Events;

namespace Ihjezly.Domain.Reviews;

public sealed class Review : Entity
{
    public Guid PropertyId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid BookingId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Review() : base(Guid.NewGuid()) { }

    private Review(Guid propertyId, Guid userId, Guid booking, int rating, string comment) : this()
    {
        if (rating < 1 || rating > 5) throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(comment)) throw new ArgumentException("Review comment cannot be empty.");

        PropertyId = propertyId;
        UserId = userId;
        BookingId = booking;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReviewCreatedDomainEvent(Id, PropertyId, UserId, Rating, Comment, CreatedAt));
    }

    public static Review Create(Guid propertyId, Guid userId,Guid bookingId, int rating, string comment)
        => new Review(propertyId, userId, bookingId, rating, comment);

    public void Update(int rating, string comment)
    {
        if (rating < 1 || rating > 5) throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(comment)) throw new ArgumentException("Review comment cannot be empty.");

        Rating = rating;
        Comment = comment;
        RaiseDomainEvent(new ReviewUpdatedDomainEvent(Id, PropertyId, UserId, Rating, Comment));
    }

    public void Delete()
    {
        RaiseDomainEvent(new ReviewDeletedDomainEvent(Id, PropertyId, UserId));
    }
}