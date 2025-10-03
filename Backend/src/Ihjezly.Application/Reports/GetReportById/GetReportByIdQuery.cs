using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.NewFolder;

namespace Ihjezly.Application.Reports.GetReportById;

public sealed record GetReportByIdQuery(Guid ReportId) : IQuery<ReportDetailsResponse>;

public sealed record ReportDetailsResponse(
    Guid Id,
    string Reason,
    string Content,
    string Replay,
    DateTime CreatedAt,
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    bool IsRead
);