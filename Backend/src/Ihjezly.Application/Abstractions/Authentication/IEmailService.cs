namespace Ihjezly.Application.Abstractions.Authentication;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

