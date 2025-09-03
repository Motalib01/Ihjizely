using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.SavedProperties.SaveProperty;

public sealed record SavePropertyCommand(Guid UserId, Guid PropertyId) : ICommand<Guid>;