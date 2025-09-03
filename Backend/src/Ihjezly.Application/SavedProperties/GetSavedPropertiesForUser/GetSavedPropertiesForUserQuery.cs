using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.SavedProperties.DTO;

namespace Ihjezly.Application.SavedProperties.GetSavedPropertiesForUser;
public sealed record GetSavedPropertiesForUserQuery(Guid UserId) : IQuery<List<SavedPropertyDto>>;