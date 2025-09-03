using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Subscriptions;

public sealed class Subscription : Entity
{
    public Guid BusinessOwnerId { get; private set; }
    public Guid PlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Money Price { get; private set; }

    public int MaxAds { get; private set; }            
    public int UsedAds { get; private set; } = 0;       

    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    public bool HasAdQuota => UsedAds < MaxAds;

    private Subscription() : base(Guid.NewGuid()) { }

    private Subscription(
        Guid businessOwnerId,
        Guid planId,
        DateTime startDate,
        DateTime endDate,
        Money price,
        int maxAds) : this()
    {
        BusinessOwnerId = businessOwnerId;
        PlanId = planId;
        StartDate = startDate;
        EndDate = endDate;
        Price = price;
        MaxAds = maxAds;

        RaiseDomainEvent(new SubscriptionCreatedDomainEvent(
            Id, BusinessOwnerId, PlanId, StartDate, EndDate, Price));
    }

    public static Subscription Create(Guid businessOwnerId, SubscriptionPlan plan, DateTime startDate)
    {
        var endDate = startDate.Add(plan.Duration);
        return new Subscription(businessOwnerId, plan.Id, startDate, endDate, plan.Price, plan.MaxAds);
    }

    public void Renew(SubscriptionPlan plan)
    {
        EndDate = EndDate.Add(plan.Duration);
        Price = plan.Price;
        MaxAds = plan.MaxAds;
        UsedAds = 0; // Optionally reset quota on renewal

        RaiseDomainEvent(new SubscriptionRenewedDomainEvent(
            Id, BusinessOwnerId, PlanId, EndDate, Price));
    }

    public void ConsumeAdQuota()
    {
        if (!HasAdQuota)
            throw new Exception("Ad quota exceeded for this subscription.");

        UsedAds++;
    }
}
