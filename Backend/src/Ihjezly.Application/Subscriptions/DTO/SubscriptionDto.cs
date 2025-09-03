namespace Ihjezly.Application.Subscriptions.DTO;

public sealed record SubscriptionDto(
    Guid Id,
    Guid BusinessOwnerId,
    Guid PlanId,
    DateTime StartDate,
    DateTime EndDate,
    decimal Price,
    string Currency,
    bool IsActive
);