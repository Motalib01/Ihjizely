using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Reposrts.Repositories;

namespace Ihjezly.Application.Reports.UpdateReport;

internal sealed class UpdateReportHandler : ICommandHandler<UpdateReportCommand>
{
    private readonly IReportRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReportHandler(IReportRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _repository.GetByIdAsync(request.ReportId, cancellationToken);
        if (report is null) return Result.Failure(ReportErrors.NotFound);

        report.Update(request.Reason, request.Content, request.IsRead);
        _repository.Update(report);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}