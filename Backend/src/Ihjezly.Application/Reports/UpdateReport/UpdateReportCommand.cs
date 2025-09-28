using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Reports.UpdateReport;

public sealed record UpdateReportCommand(Guid ReportId, string Reason, string Content, bool IsRead) : ICommand;