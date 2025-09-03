using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Notifications;

public static class NotificationErrors
{
    public static readonly Error NotFound = new(
        "Notification.NotFound",
        "The notification with the specified identifier was not found.");
}