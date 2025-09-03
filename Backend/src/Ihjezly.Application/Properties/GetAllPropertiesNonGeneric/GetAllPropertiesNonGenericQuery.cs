using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;

namespace Ihjezly.Application.Properties.GetAllPropertiesNonGeneric;

public sealed record GetAllPropertiesNonGenericQuery() : IQuery<List<PropertyDto>>;