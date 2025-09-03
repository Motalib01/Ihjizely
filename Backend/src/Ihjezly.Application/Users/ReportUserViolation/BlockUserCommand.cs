using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Users.ReportUserViolation;

public sealed record BlockUserCommand(Guid UserId, string Reason) : ICommand;