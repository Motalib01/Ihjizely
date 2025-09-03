using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Reports.DeleteReport;

public sealed record DeleteReportCommand(Guid ReportId) : ICommand;