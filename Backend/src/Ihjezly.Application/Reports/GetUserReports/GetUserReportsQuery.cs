using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Reposrts.Repositories;

namespace Ihjezly.Application.Reports.GetUserReports;

public sealed record GetUserReportsQuery(Guid UserId) : IQuery<IReadOnlyList<UserReportResponse>>;

public sealed record UserReportResponse(
    Guid Id,
    string Reason,
    string Content,
    string? Replay,
    bool IsRead,
    DateTime CreatedAt
);

public sealed class GetUserReportsQueryHandler
    : IQueryHandler<GetUserReportsQuery, IReadOnlyList<UserReportResponse>>
{
    private readonly IReportRepository _reportRepository;

    public GetUserReportsQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Result<IReadOnlyList<UserReportResponse>>> Handle(
        GetUserReportsQuery request,
        CancellationToken cancellationToken)
    {
        var allReports = await _reportRepository.GetAllAsync(cancellationToken);

        var userReports = allReports
            .Where(r => r.UserId == request.UserId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new UserReportResponse(
                r.Id,
                r.Reason,
                r.Content,
                r.Replay,
                r.IsRead,
                r.CreatedAt
            ))
            .ToList();

        return userReports;
    }
}