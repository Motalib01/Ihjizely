using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // Find the user by phone number
        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        // Check that new password and confirmation match
        if (request.NewPassword != request.ConfirmNewPassword)
            return Result.Failure(UserErrors.PasswordConfirmationMismatch);

        // Hash the new password
        var hashedPassword = _jwtService.HashPassword(request.NewPassword);

        // Update user password
        user.ChangePassword(hashedPassword);
        _userRepository.Update(user);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}