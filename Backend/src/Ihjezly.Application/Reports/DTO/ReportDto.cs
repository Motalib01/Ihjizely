namespace Ihjezly.Application.Reports.DTO;

public sealed record ReportDto(
    Guid Id,
    Guid UserId,
    string Reason,
    string Content,
    DateTime CreatedAt
);