using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Subscriptions.ActivateSubscriptionPlan;

public sealed record ActivateSubscriptionPlanCommand(Guid PlanId, Guid UserId) : ICommand;