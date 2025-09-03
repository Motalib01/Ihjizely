using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;

namespace Ihjezly.Application.Properties.GetPropertyByIdNonGeneric;


public sealed record GetPropertyByIdNonGenericQuery(Guid Id) : IQuery<PropertyDto>;