using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Properties;

namespace Ihjezly.Application.Properties.ChangePropertyStatusNonGeneric;

public sealed record ChangePropertyStatusNonGenericCommand(Guid PropertyId, PropertyStatus NewStatus) : ICommand;