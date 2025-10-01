using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using Ihjezly.Application.Abstractions.Authentication;

namespace Ihjezly.Infrastructure.Authentication;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var host = _config["Smtp:Host"];
        var port = int.Parse(_config["Smtp:Port"]);
        var username = _config["Smtp:Username"];
        var password = _config["Smtp:Password"];
        var from = _config["Smtp:From"];

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        using var message = new MailMessage(from, to, subject, body);

        try
        {
            await client.SendMailAsync(message);
        }
        catch (SmtpException smtpEx)
        {
            throw new InvalidOperationException($"SMTP error: {smtpEx.StatusCode} - {smtpEx.Message}", smtpEx);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unexpected error while sending email: {ex.Message}", ex);
        }
    }


}