using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Subscriptions.UpdateSubscriptionPlan;

public sealed record UpdateSubscriptionPlanCommand(
    Guid PlanId,
    string Name,
    int DurationInDays,   
    decimal Amount,
    string Currency,
    int MaxAds
) : ICommand;

