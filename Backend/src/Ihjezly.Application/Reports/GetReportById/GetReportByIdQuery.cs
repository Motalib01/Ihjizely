using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.NewFolder;

namespace Ihjezly.Application.Reports.GetReportById;

public sealed record GetReportByIdQuery(Guid ReportId) : IQuery<Report>;