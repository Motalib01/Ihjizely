using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.ReportUserViolation;

internal sealed class UnblockUserCommandHandler : ICommandHandler<UnblockUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnblockUserCommandHandler(
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        user.Unblock();
        _userRepository.Update(user);

        var notificationMessage = "Your account has been unblocked. You can now access all features again.";
        _notificationRepository.Add(Notification.Create(user.Id, notificationMessage));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}