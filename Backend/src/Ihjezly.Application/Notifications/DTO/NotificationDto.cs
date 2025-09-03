namespace Ihjezly.Application.Notifications.DTO;

public sealed record NotificationDto(
    Guid Id,
    Guid UserId,
    string Message,
    DateTime SentAt,
    bool IsRead
);