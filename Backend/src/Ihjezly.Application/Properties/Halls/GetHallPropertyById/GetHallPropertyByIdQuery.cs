using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;

namespace Ihjezly.Application.Properties.Halls.GetHallPropertyById;

public sealed record GetHallPropertyByIdQuery(Guid PropertyId)
    : IQuery<HallPropertyDto>;