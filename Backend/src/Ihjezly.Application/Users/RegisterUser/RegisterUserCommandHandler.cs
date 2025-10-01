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
        var existingUser = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (existingUser is not null)
            return Result.Failure<UserDto>(UserErrors.PhoneNumberAlreadyInUse);

        var existingEmail = await _userRepository.GetByPhoneOrEmailAsync(request.Email, cancellationToken);
        if (existingEmail is not null)
            return Result.Failure<UserDto>(UserErrors.PhoneNumberAlreadyInUse);



        var hashedPassword = _jwtService.HashPassword(request.Password);

        User user = request.Role switch
        {
            UserRole.Client => Client.Create(request.FirstName, request.LastName, request.PhoneNumber, request.Email, hashedPassword, false),
            UserRole.BusinessOwner => BusinessOwner.Create(request.FirstName, request.LastName, request.PhoneNumber, request.Email, hashedPassword, false),
            UserRole.Admin => Admin.Create(request.FirstName, request.LastName, request.PhoneNumber, request.Email, hashedPassword,  false),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Role), $"Unsupported role: {request.Role}")
        };

        await _userRepository.AddAsync(user, cancellationToken);

        //Create a wallet for every user role
        var wallet = Wallet.Create(user.Id);
        _walletRepository.Add(wallet);

        // Raise domain event (only once — don't publish manually)
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user.Role));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToDto());
    }

}
