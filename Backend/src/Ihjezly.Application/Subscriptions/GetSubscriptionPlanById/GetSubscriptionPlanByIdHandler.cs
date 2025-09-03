using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Subscriptions.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Subscriptions.GetSubscriptionPlanById;

internal sealed class GetSubscriptionPlanByIdHandler : IQueryHandler<GetSubscriptionPlanByIdQuery, SubscriptionPlanDto>
{
    private readonly ISubscriptionPlanRepository _repository;

    public GetSubscriptionPlanByIdHandler(ISubscriptionPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SubscriptionPlanDto>> Handle(GetSubscriptionPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null) return Result.Failure<SubscriptionPlanDto>(SubscriptionErrors.PlanNotFound);

        return new SubscriptionPlanDto(plan.Id, plan.Name, plan.Duration, plan.Price.Amount, plan.Price.Currency.Code, plan.IsActive, plan.MaxAds);
    }
}