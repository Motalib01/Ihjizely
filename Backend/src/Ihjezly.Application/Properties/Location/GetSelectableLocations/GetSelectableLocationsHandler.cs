using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.Location.GetSelectableLocations;

internal sealed class GetSelectableLocationsHandler
    : IQueryHandler<GetSelectableLocationsQuery, List<SelectableLocationDto>>
{
    private readonly ILocationRepository _repository;

    public GetSelectableLocationsHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<SelectableLocationDto>>> Handle(GetSelectableLocationsQuery query, CancellationToken cancellationToken)
    {
        var locations = await _repository.GetAllAsync(cancellationToken);
        var result = locations
            .Select(l => new SelectableLocationDto(l.Id, l.City, l.State, l.Country))
            .ToList();

        return Result.Success(result);
    }
}