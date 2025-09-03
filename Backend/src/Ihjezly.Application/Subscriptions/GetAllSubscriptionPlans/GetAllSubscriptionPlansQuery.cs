using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Subscriptions.DTO;

namespace Ihjezly.Application.Subscriptions.GetAllSubscriptionPlans;

public sealed record GetAllSubscriptionPlansQuery() : IQuery<List<SubscriptionPlanDto>>;