using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.ChangePropertyStatus;

public sealed record ChangePropertyStatusCommand(Guid PropertyId, PropertyStatus NewStatus)
    : ICommand<Result>;