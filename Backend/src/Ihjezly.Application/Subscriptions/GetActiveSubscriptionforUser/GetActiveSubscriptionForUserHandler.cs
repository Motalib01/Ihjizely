using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Subscriptions.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Subscriptions.GetActiveSubscriptionforUser;

internal sealed class GetActiveSubscriptionForUserHandler
    : IQueryHandler<GetActiveSubscriptionForUserQuery, SubscriptionDto>
{
    private readonly ISubscriptionRepository _repository;

    public GetActiveSubscriptionForUserHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SubscriptionDto>> Handle(GetActiveSubscriptionForUserQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _repository.GetByBusinessOwnerIdAsync(request.BusinessOwnerId, cancellationToken);

        if (subscriptions is null || !subscriptions.Any())
            return Result.Failure<SubscriptionDto>(SubscriptionErrors.SubscriptionNotFound);

        var activeSubscription = subscriptions.FirstOrDefault(x => x.IsActive);
        if (activeSubscription is null)
            return Result.Failure<SubscriptionDto>(SubscriptionErrors.ActiveSubscriptionNotFound);

        return new SubscriptionDto(
            activeSubscription.Id,
            activeSubscription.BusinessOwnerId,
            activeSubscription.PlanId,
            activeSubscription.StartDate,
            activeSubscription.EndDate,
            activeSubscription.Price.Amount,
            activeSubscription.Price.Currency.Code,
            activeSubscription.IsActive,
            activeSubscription.UsedAds,
            activeSubscription.MaxAds
        );
    }


}