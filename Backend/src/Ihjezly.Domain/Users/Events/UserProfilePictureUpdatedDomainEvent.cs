using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Users.Events;

public sealed record UserProfilePictureUpdatedDomainEvent(Guid UserId, string PictureUrl) : IDomainEvent;