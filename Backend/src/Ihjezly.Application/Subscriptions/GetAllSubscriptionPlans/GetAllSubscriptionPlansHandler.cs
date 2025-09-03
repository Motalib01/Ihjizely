using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Subscriptions.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Subscriptions.GetAllSubscriptionPlans;

internal sealed class GetAllSubscriptionPlansHandler
    : IQueryHandler<GetAllSubscriptionPlansQuery, List<SubscriptionPlanDto>>
{
    private readonly ISubscriptionPlanRepository _repository;

    public GetAllSubscriptionPlansHandler(ISubscriptionPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<SubscriptionPlanDto>>> Handle(
        GetAllSubscriptionPlansQuery query,
        CancellationToken cancellationToken)
    {
        var plans = await _repository.GetAllAsync(cancellationToken);

        var dtos = plans.Select(plan => new SubscriptionPlanDto(
            plan.Id,
            plan.Name,
            plan.Duration,
            plan.Price.Amount,
            plan.Price.CurrencyCode,
            plan.IsActive,
            plan.MaxAds
        )).ToList();

        return Result.Success(dtos);
    }
}