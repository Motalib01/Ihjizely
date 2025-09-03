using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Notifications.DTO;

namespace Ihjezly.Application.Notifications.ListUserNotifications;

public sealed record ListUserNotificationsQuery(Guid UserId) : IQuery<List<NotificationDto>>;