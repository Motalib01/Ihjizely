using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Users.LogInUser;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AccessTokenResponse>
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(
        IJwtService jwtService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccessTokenResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

        // Check if user exists
        if (user is null)
        {
            return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);
        }

        // Check if user is blocked (manually by Admin)
        if (user.IsBlocked)
        {
            return Result.Failure<AccessTokenResponse>(UserErrors.UserBlocked);
        }

        // Verify password
        if (!_jwtService.VerifyPassword(user.Password, request.Password))
        {
            return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);
        }

        // Login success → generate JWT
        var token = _jwtService.GenerateToken(user);
        return new AccessTokenResponse(token);
    }
}
