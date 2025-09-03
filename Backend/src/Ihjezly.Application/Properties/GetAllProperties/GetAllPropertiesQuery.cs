using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;

namespace Ihjezly.Application.Properties.GetAllProperties;

public sealed record GetAllPropertiesQuery : IQuery<List<PropertyDto>>;