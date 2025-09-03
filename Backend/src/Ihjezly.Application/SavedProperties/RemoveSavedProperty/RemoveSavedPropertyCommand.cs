using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.SavedProperties.RemoveSavedProperty;


public sealed record RemoveSavedPropertyCommand(Guid SavedPropertyId, Guid UserId) : ICommand;
