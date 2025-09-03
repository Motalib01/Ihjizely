namespace Ihjezly.Application.Reviews.DTO;

public sealed record ReviewDto(
    Guid Id,
    Guid PropertyId,
    Guid UserId,
    int Rating,
    string Comment,
    DateTime CreatedAt
);