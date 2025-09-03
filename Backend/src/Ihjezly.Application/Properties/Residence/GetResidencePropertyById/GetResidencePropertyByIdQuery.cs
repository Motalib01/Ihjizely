using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.GetPropertyByIdGeneric;

public sealed record GetResidencePropertyByIdQuery<TProperty, TDetails>(Guid PropertyId)
    : IQuery<ResidencePropertyDto>
    where TProperty : ResidenceProperty<TDetails>;