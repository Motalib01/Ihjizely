using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.DeleteProperty;

public sealed record DeletePropertyCommand<TProperty, TDetails>(Guid PropertyId) : ICommand
    where TProperty : PropertyWithDetails<TDetails>, new();