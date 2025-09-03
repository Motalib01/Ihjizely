using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;

namespace Ihjezly.Application.Properties.Location.GetSelectableLocations;

public sealed record GetSelectableLocationsQuery() : IQuery<List<SelectableLocationDto>>;