using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Subscriptions.DeleteSubscriptionPlan;

public sealed record DeleteSubscriptionPlanCommand(Guid PlanId) : ICommand<Result>;

internal sealed class DeleteSubscriptionPlanCommandHandler
    : ICommandHandler<DeleteSubscriptionPlanCommand, Result>
{
    private readonly ISubscriptionPlanRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSubscriptionPlanCommandHandler(
        ISubscriptionPlanRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Result>> Handle(DeleteSubscriptionPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
            return Result.Failure(SubscriptionPlanErrors.NotFound);

        _repository.Remove(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}