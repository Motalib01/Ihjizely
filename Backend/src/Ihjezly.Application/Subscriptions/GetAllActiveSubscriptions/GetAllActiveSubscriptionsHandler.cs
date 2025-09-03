using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Subscriptions.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Subscriptions.GetAllActiveSubscriptions;

internal sealed class GetAllActiveSubscriptionsHandler
    : IQueryHandler<GetAllActiveSubscriptionsQuery, List<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _repository;

    public GetAllActiveSubscriptionsHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<SubscriptionDto>>> Handle(GetAllActiveSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _repository.GetAllAsync(cancellationToken);

        var activeSubscriptions = subscriptions
            .Where(x => x.IsActive)
            .Select(x => new SubscriptionDto(
                x.Id,
                x.BusinessOwnerId,
                x.PlanId,
                x.StartDate,
                x.EndDate,
                x.Price.Amount,
                x.Price.Currency.Code,
                x.IsActive
            ))
            .ToList();

        return activeSubscriptions;
    }
}