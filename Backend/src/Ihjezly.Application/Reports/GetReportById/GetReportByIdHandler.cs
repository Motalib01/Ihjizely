using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Reposrts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Application.Reports.GetReportById
{
    internal sealed class GetReportByIdHandler
        : IQueryHandler<GetReportByIdQuery, ReportDetailsResponse>
    {
        private readonly IReportRepository _repository;

        public GetReportByIdHandler(IReportRepository repository)
            => _repository = repository;

        public async Task<Result<ReportDetailsResponse>> Handle(
            GetReportByIdQuery request,
            CancellationToken cancellationToken)
        {
            var report = await _repository
                .GetByIdWithUserAsync(request.ReportId, cancellationToken);
            // <-- you’ll need this method in repo

            if (report is null)
                return Result.Failure<ReportDetailsResponse>(ReportErrors.NotFound);

            var response = new ReportDetailsResponse(
                report.Id,
                report.Reason,
                report.Content,
                report.Replay,
                report.CreatedAt,
                report.User.Id,
                report.User.FirstName,
                report.User.LastName,
                report.User.PhoneNumber,
                report.User.Email,
                report.IsRead
            );

            return response;
        }
    }
}