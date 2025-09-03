using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Properties.Location.CreateLocation;

public sealed record CreateLocationCommand(string City, string State, string Country) : ICommand<Guid>;