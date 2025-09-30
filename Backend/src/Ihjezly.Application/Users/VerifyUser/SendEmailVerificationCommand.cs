using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.VerifyUser;

public record SendEmailVerificationCommand(Guid UserId, string Email) : ICommand;


internal sealed class SendEmailVerificationHandler
    : ICommandHandler<SendEmailVerificationCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationRepository _otpRepository;
    private readonly IEmailSender _emailSender;
    private readonly IUnitOfWork _unitOfWork;

    public SendEmailVerificationHandler(
        IUserRepository userRepository,
        IEmailVerificationRepository otpRepository,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _otpRepository = otpRepository;
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        // Generate OTP
        var otp = new Random().Next(100000, 999999).ToString();

        // Save OTP
        var otpEntity = EmailVerificationCode.Create(request.UserId, otp, DateTime.UtcNow.AddMinutes(5));
        await _otpRepository.AddAsync(otpEntity, cancellationToken);

        // Send email
        await _emailSender.SendAsync(
            request.Email,
            "Verify your email",
            $"Your verification code is {otp}");

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


public record SendOtpRequest(Guid UserId, string Email);