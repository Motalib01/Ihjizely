using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;

namespace Ihjezly.Application.Properties.GetPropertiesByBusinessOwnerId;

public sealed record GetPropertiesByBusinessOwnerIdQuery(Guid BusinessOwnerId)
    : IQuery<List<PropertyDto>>;