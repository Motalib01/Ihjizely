using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Users.ReportUserViolation;

public sealed record UnblockUserCommand(Guid UserId) : ICommand;