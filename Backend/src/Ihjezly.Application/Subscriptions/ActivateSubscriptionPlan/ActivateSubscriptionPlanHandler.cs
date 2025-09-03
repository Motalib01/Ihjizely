using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Subscriptions.ActivateSubscriptionPlan;

internal sealed class ActivateSubscriptionPlanHandler
    : ICommandHandler<ActivateSubscriptionPlanCommand>
{
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateSubscriptionPlanHandler(
        ISubscriptionPlanRepository subscriptionPlanRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _subscriptionPlanRepository = subscriptionPlanRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ActivateSubscriptionPlanCommand request, CancellationToken cancellationToken)
    {
        // 1. Get the plan
        var plan = await _subscriptionPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
            return Result.Failure(SubscriptionErrors.PlanNotFound);

        // 2. Get the wallet for the user
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (wallet is null)
            return Result.Failure(WalletErrors.NotFound);

        // 3. Deduct the funds
        try
        {
            wallet.DeductFunds(plan.Price); // assuming plan.Price is Money
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(WalletErrors.InsufficientBalance);
        }

        // 4. Activate the plan
        plan.Activate();

        // 5. Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
