using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.GetPropertyById;

public sealed record GetPropertyByIdQuery<TProperty, TDetails>(Guid Id)
    : IQuery<PropertyDto?>
    where TProperty : PropertyWithDetails<TDetails>;