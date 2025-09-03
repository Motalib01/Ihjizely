using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.GetAllPropertiesByType;

public sealed record GetAllPropertiesByStatusQuery(PropertyStatus Status) : IQuery<List<PropertyDto>>;

// Handler