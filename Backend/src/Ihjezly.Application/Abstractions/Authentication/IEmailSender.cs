namespace Ihjezly.Application.Abstractions.Authentication;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body);
}