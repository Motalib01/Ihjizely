using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.Location.GetLocationById;

internal sealed class GetLocationByIdHandler
    : IQueryHandler<GetLocationByIdQuery, SelectableLocationDto>
{
    private readonly ILocationRepository _repository;

    public GetLocationByIdHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SelectableLocationDto>> Handle(GetLocationByIdQuery query, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(query.Id, cancellationToken);

        if (location is null)
            return Result.Failure<SelectableLocationDto>(PropertyErrors.NotFound);

        var dto = new SelectableLocationDto(location.Id, location.City, location.State, location.Country);
        return Result.Success(dto);
    }
}