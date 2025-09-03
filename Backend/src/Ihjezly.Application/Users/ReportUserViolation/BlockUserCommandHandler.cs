using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.ReportUserViolation;

internal sealed class BlockUserCommandHandler : ICommandHandler<BlockUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BlockUserCommandHandler(
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        user.Block(request.Reason);

        _userRepository.Update(user);

        // Send a notification to the blocked user
        var notificationMessage = $"Your account has been blocked for the following reason: {request.Reason}";
        _notificationRepository.Add(Notification.Create(user.Id, notificationMessage));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}