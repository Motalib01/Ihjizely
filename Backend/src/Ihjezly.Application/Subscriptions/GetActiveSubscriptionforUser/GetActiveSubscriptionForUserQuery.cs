using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Subscriptions.DTO;

namespace Ihjezly.Application.Subscriptions.GetActiveSubscriptionforUser;

public sealed record GetActiveSubscriptionForUserQuery(Guid BusinessOwnerId) : IQuery<SubscriptionDto>;