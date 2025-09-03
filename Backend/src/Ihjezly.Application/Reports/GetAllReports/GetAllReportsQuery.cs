using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Reports.DTO;

namespace Ihjezly.Application.Reports.GetAllReports;

public sealed record GetAllReportsQuery : IQuery<IReadOnlyList<ReportDto>>;