using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;

namespace Ihjezly.Application.Properties.Location.GetLocationById;

public sealed record GetLocationByIdQuery(Guid Id) : IQuery<SelectableLocationDto>;