using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Reposrts.Repositories;

namespace Ihjezly.Application.Reports.ReplayReport;

public sealed record ReplayReportCommand(
    Guid ReportId,
    string Replay
) : ICommand;

internal sealed class ReplayReportCommandHandler
    : ICommandHandler<ReplayReportCommand>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReplayReportCommandHandler(
        IReportRepository reportRepository,
        IUnitOfWork unitOfWork)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReplayReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.ReportId, cancellationToken);
        if (report is null)
            return Result.Failure(new Error("Report.NotFound", "Report not found."));

        report.AddReplay(request.Replay);

        _reportRepository.Update(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}