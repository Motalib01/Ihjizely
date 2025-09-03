using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Reports.CreateReport;

public sealed record CreateReportCommand(Guid UserId, string Reason, string Content) : ICommand;