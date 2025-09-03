using Ihjezly.Domain.Subscriptions;
using MediatR;

namespace Ihjezly.Application.Subscriptions.GetAllSubscriptions;

public sealed class GetAllSubscriptionsQueryHandler : IRequestHandler<GetAllSubscriptionsQuery, List<Subscription>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetAllSubscriptionsQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<List<Subscription>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync(cancellationToken);
        return subscriptions;
    }
}