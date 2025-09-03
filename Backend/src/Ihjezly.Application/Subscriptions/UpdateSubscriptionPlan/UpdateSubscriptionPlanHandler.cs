using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Subscriptions.UpdateSubscriptionPlan;

internal sealed class UpdateSubscriptionPlanHandler : ICommandHandler<UpdateSubscriptionPlanCommand>
{
    private readonly ISubscriptionPlanRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSubscriptionPlanHandler(ISubscriptionPlanRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSubscriptionPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
            return Result.Failure(SubscriptionErrors.PlanNotFound);

        var money = new Money(request.Amount, Currency.FromCode(request.Currency));

        // Use days instead of TimeSpan
        plan.Update(
            name: request.Name,
            durationInDays: request.DurationInDays, 
            price: money,
            maxAds: request.MaxAds
        );

        _repository.Update(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}