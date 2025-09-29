using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Reposrts.Repositories;

namespace Ihjezly.Application.Reports.MarkReportAsRead;

public sealed record MarkReportAsReadCommand(Guid ReportId) : ICommand<Result>;

internal sealed class MarkReportAsReadHandler
    : ICommandHandler<MarkReportAsReadCommand, Result>
{
    private readonly IReportRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkReportAsReadHandler(
        IReportRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Result>> Handle(
        MarkReportAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var report = await _repository.GetByIdAsync(request.ReportId, cancellationToken);

        if (report is null)
            return Result.Failure(ReportErrors.NotFound);

        report.MarkAsRead(); // domain method in Report entity (recommended)

        _repository.Update(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}