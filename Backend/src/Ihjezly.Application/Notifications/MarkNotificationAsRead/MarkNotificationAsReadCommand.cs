using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Notifications.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(Guid NotificationId) : ICommand;