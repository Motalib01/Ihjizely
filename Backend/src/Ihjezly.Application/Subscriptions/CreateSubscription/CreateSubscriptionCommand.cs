using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Subscriptions.CreateSubscription;

public sealed record CreateSubscriptionCommand(
    Guid BusinessOwnerId,
    Guid PlanId,
    DateTime StartDate
) : ICommand<Guid>;