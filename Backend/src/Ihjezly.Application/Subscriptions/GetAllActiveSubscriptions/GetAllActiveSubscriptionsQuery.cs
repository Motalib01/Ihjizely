using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Subscriptions.DTO;

namespace Ihjezly.Application.Subscriptions.GetAllActiveSubscriptions;

public sealed record GetAllActiveSubscriptionsQuery()
    : IQuery<List<SubscriptionDto>>;