using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Notifications.CreateNotification;

public sealed record CreateNotificationCommand(Guid UserId, string Message) : ICommand<Guid>;