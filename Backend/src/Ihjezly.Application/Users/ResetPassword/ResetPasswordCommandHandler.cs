using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.ResetPassword;

internal sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        var isOldPasswordValid = _jwtService.VerifyPassword(user.Password, request.OldPassword);

        if (!isOldPasswordValid)
            return Result.Failure(UserErrors.InvalidPassword);

        if (request.NewPassword != request.ConfirmNewPassword)
            return Result.Failure(UserErrors.PasswordConfirmationMismatch);

        var newPasswordHash = _jwtService.HashPassword(request.NewPassword);
        user.ChangePassword(newPasswordHash);

        // Add security notification
        _notificationRepository.Add(
            Notification.Create(
                user.Id,
                "Your password was changed successfully. If this wasn’t you, please contact support immediately."
            )
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
