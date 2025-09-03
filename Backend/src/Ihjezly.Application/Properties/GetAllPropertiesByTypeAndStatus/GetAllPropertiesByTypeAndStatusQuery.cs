using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.GetPropertiesByStatus;

public sealed record GetAllPropertiesByTypeAndStatusQuery<TProperty, TDetails>(PropertyStatus Status)
    : IQuery<List<PropertyDto>>
    where TProperty : PropertyWithDetails<TDetails>;