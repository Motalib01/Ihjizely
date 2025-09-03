using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Reposrts.Repositories;

namespace Ihjezly.Application.Reports.CreateReport;

internal sealed class CreateReportHandler : ICommandHandler<CreateReportCommand>
{
    private readonly IReportRepository _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReportHandler(
        IReportRepository repository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = Report.Create(request.UserId, request.Reason, request.Content);
        _repository.Add(report);

        var notificationMessage = "تم إرسال بلاغك بنجاح وهو قيد المراجعة.";
        var notification = Notification.Create(request.UserId, notificationMessage);
        _notificationRepository.Add(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}