namespace Ihjezly.Application.SavedProperties.DTO;

public sealed record SavedPropertyDto(Guid Id, Guid UserId, Guid PropertyId, DateTime SavedAt);