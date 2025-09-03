using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Subscriptions.RenewSubscription;

public sealed record RenewSubscriptionCommand(Guid SubscriptionId, Guid PlanId) : ICommand;