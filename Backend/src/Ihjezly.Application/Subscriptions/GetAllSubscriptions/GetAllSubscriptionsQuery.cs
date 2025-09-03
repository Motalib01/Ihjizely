using Ihjezly.Domain.Subscriptions;
using MediatR;

namespace Ihjezly.Application.Subscriptions.GetAllSubscriptions;

public record GetAllSubscriptionsQuery() : IRequest<List<Subscription>>;