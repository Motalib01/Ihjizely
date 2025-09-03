using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.GetAllPropertiesByType;

public sealed record GetAllPropertiesByTypeQuery<TProperty, TDetails>() : IQuery<List<PropertyDto>>
    where TProperty : PropertyWithDetails<TDetails>;