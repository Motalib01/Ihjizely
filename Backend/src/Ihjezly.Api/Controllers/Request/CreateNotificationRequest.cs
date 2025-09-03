namespace Ihjezly.Api.Controllers.Request;

public sealed class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}