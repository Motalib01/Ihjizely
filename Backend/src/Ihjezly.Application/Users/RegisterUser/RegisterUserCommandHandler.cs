using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Users.Common;
using Ihjezly.Application.Users.RegisterUser;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Events;
using Ihjezly.Domain.Users.Repositories;
using MediatR;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IWalletRepository _walletRepository;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IWalletRepository walletRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _walletRepository = walletRepository;
    }

    public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var existingUser = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            if (existingUser is not null)
                return Result.Failure<UserDto>(UserErrors.PhoneNumberAlreadyInUse);
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUserByEmail is not null)
                return Result.Failure<UserDto>(UserErrors.EmailAlreadyInUse);
        }

        var hashedPassword = _jwtService.HashPassword(request.Password);

        User user = request.Role switch
        {
            UserRole.Client => Client.Create( request.FullName, request.PhoneNumber, request.Email, hashedPassword, false),
            UserRole.BusinessOwner => BusinessOwner.Create(request.FullName, request.PhoneNumber, request.Email, hashedPassword, false),
            UserRole.Admin => Admin.Create( request.FullName, request.PhoneNumber, request.Email, hashedPassword, false),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Role), $"Unsupported role: {request.Role}")
        };

        await _userRepository.AddAsync(user, cancellationToken);

        var wallet = Wallet.Create(user.Id);
        _walletRepository.Add(wallet);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user.Role));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToDto());
    }


}
