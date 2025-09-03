using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Users.ForgotPassword;

public sealed record ForgotPasswordCommand(string PhoneNumber, string NewPassword, string ConfirmNewPassword) : ICommand;