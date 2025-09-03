using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Reposrts.Repositories;

namespace Ihjezly.Application.Reports.GetReportById;

internal sealed class GetReportByIdHandler : IQueryHandler<GetReportByIdQuery, Report>
{
    private readonly IReportRepository _repository;

    public GetReportByIdHandler(IReportRepository repository) => _repository = repository;

    public async Task<Result<Report>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = await _repository.GetByIdAsync(request.ReportId, cancellationToken);
        return report is null ? Result.Failure<Report>(ReportErrors.NotFound) : report;
    }
}