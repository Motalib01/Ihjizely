using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Properties.DeleteProperty;

public sealed record DeletePropertyNonGenericCommand(Guid PropertyId) : ICommand;