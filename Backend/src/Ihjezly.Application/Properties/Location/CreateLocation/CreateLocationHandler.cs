using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.Location.CreateLocation;

internal sealed class CreateLocationHandler : ICommandHandler<CreateLocationCommand, Guid>
{
    private readonly ILocationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLocationHandler(ILocationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var location = new SelectableLocation(command.City, command.State, command.Country);
        _repository.Add(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(location.Id);
    }
}