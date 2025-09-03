using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Properties.Location.DeleteLocation;

public sealed record DeleteLocationCommand(Guid Id) : ICommand;