using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Reports.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Reposrts.Repositories;

namespace Ihjezly.Application.Reports.GetAllReports;

internal sealed class GetAllReportsHandler : IQueryHandler<GetAllReportsQuery, IReadOnlyList<ReportDto>>
{
    private readonly IReportRepository _repository;

    public GetAllReportsHandler(IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<ReportDto>>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _repository.GetAllAsync(cancellationToken);

        var result = reports
            .Select(r => new ReportDto(
                r.Id,
                r.UserId,
                r.Reason,
                r.Content,
                r.CreatedAt))
            .ToList();

        return result;
    }
}