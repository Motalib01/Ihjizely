using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Reposrts.Repositories;

namespace Ihjezly.Application.Reports.DeleteReport;

internal sealed class DeleteReportHandler : ICommandHandler<DeleteReportCommand>
{
    private readonly IReportRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReportHandler(IReportRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _repository.GetByIdAsync(request.ReportId, cancellationToken);
        if (report is null) return Result.Failure(ReportErrors.NotFound);

        report.Delete();
        _repository.Delete(report);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}