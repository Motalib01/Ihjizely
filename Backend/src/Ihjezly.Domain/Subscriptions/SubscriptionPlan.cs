using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

public sealed class SubscriptionPlan : Entity
{
    public string Name { get; private set; }

    // Store duration as days in DB to avoid overflow
    public int DurationInDays { get; private set; }

    // Domain-only property for convenience
    public TimeSpan Duration => TimeSpan.FromDays(DurationInDays);

    public Money Price { get; private set; }
    public bool IsActive { get; private set; }
    public int MaxAds { get; private set; }

    private SubscriptionPlan() : base(Guid.NewGuid()) { }

    private SubscriptionPlan(string name, int durationInDays, Money price, int maxAds) : this()
    {
        Name = name;
        DurationInDays = durationInDays;
        Price = price;
        MaxAds = maxAds;
        IsActive = true;

        RaiseDomainEvent(new SubscriptionPlanCreatedDomainEvent(Id, Name, Duration, Price));
    }

    public static SubscriptionPlan Create(string name, int durationInDays, Money price, int maxAds)
        => new(name, durationInDays, price, maxAds);

    public void Update(string name, int durationInDays, Money price, int maxAds)
    {
        Name = name;
        DurationInDays = durationInDays;
        Price = price;
        MaxAds = maxAds;

        RaiseDomainEvent(new SubscriptionPlanUpdatedDomainEvent(Id, Name, Duration, Price));
    }

    public void Activate()
    {
        IsActive = true;
        RaiseDomainEvent(new SubscriptionPlanActivatedDomainEvent(Id));
    }

    public void Deactivate()
    {
        IsActive = false;
        RaiseDomainEvent(new SubscriptionPlanDeactivatedDomainEvent(Id));
    }
}