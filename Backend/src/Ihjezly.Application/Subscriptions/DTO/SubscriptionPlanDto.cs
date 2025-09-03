namespace Ihjezly.Application.Subscriptions.DTO;

public sealed record SubscriptionPlanDto(
    Guid Id,
    string Name,
    TimeSpan Duration,
    decimal Amount,
    string Currency,
    bool IsActive,
    int MaxAds);