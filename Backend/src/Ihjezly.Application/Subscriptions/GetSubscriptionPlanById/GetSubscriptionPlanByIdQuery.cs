using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Subscriptions.DTO;

namespace Ihjezly.Application.Subscriptions.GetSubscriptionPlanById;

public sealed record GetSubscriptionPlanByIdQuery(Guid PlanId) : IQuery<SubscriptionPlanDto>;