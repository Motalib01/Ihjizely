using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.SavedProperties;

namespace Ihjezly.Application.SavedProperties.RemoveSavedProperty;

internal sealed class RemoveSavedPropertyHandler : ICommandHandler<RemoveSavedPropertyCommand>
{
    private readonly ISavedPropertyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveSavedPropertyHandler(ISavedPropertyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveSavedPropertyCommand request, CancellationToken cancellationToken)
    {
        var savedProperty = await _repository.GetByIdAsync(request.SavedPropertyId, cancellationToken);
        if (savedProperty is null)
            return Result.Failure(SavedPropertyErrors.NotFound);

        if (savedProperty.UserId != request.UserId)
            return Result.Failure(SavedPropertyErrors.Unauthorized); 

        savedProperty.Remove();
        _repository.Delete(savedProperty);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
