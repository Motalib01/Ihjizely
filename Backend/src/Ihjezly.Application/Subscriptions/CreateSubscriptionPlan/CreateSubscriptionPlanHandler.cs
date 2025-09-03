using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Subscriptions.CreateSubscriptionPlan;

internal sealed class CreateSubscriptionPlanHandler : ICommandHandler<CreateSubscriptionPlanCommand, Guid>
{
    private readonly ISubscriptionPlanRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubscriptionPlanHandler(ISubscriptionPlanRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateSubscriptionPlanCommand request, CancellationToken cancellationToken)
    {
        var money = new Money(request.Amount, Currency.FromCode(request.Currency));

        // Use days instead of TimeSpan
        var plan = SubscriptionPlan.Create(
            name: request.Name,
            durationInDays: request.DurationInDays, 
            price: money,
            maxAds: request.MaxAds
        );

        _repository.Add(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return plan.Id;
    }
}