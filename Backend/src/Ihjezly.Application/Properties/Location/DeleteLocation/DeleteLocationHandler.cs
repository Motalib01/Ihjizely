using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.Location.DeleteLocation;

internal sealed class DeleteLocationHandler : ICommandHandler<DeleteLocationCommand>
{
    private readonly ILocationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLocationHandler(ILocationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteLocationCommand command, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (location is null) return Result.Failure(PropertyErrors.NotFound);

        _repository.Remove(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}