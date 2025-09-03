using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Subscriptions.CreateSubscriptionPlan;

public sealed record CreateSubscriptionPlanCommand(
    string Name,
    int DurationInDays,   
    decimal Amount,
    string Currency,
    int MaxAds
) : ICommand<Guid>;

