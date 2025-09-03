using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.ReportUserViolation;

internal sealed class ReportUserViolationCommandHandler : ICommandHandler<ReportUserViolationCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReportUserViolationCommandHandler(
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReportUserViolationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        user.ReportViolation();

        var violationMessage = $"A violation has been recorded on your account. Total violations: {user.ViolationCount}.";
        _notificationRepository.Add(Notification.Create(user.Id, violationMessage));

        if (user.ViolationCount >= 3 && !user.IsBlocked)
        {
            user.Block("Violation threshold exceeded (3+)");
            var blockMessage = "Your account has been blocked due to exceeding the allowed number of violations.";
            _notificationRepository.Add(Notification.Create(user.Id, blockMessage));
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}