using Ihjezly.Application.Abstractions.Messaging;
using System.Text.Json.Serialization;

namespace Ihjezly.Application.Properties.Location.UpdateLocation;

public sealed record UpdateLocationCommand(
    [property: JsonIgnore] Guid Id,
    string City, 
    string State, 
    string Country) : ICommand;