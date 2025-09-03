using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Notifications.DeleteNotification;

public sealed record DeleteNotificationCommand(Guid NotificationId) : ICommand;