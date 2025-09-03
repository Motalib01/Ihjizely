using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.Location.UpdateLocation;

internal sealed class UpdateLocationHandler : ICommandHandler<UpdateLocationCommand>
{
    private readonly ILocationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLocationHandler(ILocationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (location is null) return Result.Failure(PropertyErrors.NotFound);

        location.Update(command.City, command.State, command.Country);
        _repository.Update(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}