using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Users.ReportUserViolation;

public sealed record ReportUserViolationCommand(Guid UserId) : ICommand;