using Ihjezly.Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Ihjezly.Infrastructure.Authentication;

internal sealed class EmailService : IEmailService
{
    private readonly string _from;
    private readonly string _password;
    private readonly string _host;
    private readonly int _port;

    public EmailService(IConfiguration configuration)
    {
        _from = configuration["Smtp:From"]!;
        _password = configuration["Smtp:Password"]!;
        _host = configuration["Smtp:Host"]!;
        _port = int.Parse(configuration["Smtp:Port"]!);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Ihjezly", _from));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_from, _password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}