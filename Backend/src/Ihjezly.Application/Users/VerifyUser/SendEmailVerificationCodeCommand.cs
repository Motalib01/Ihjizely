using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.VerifyUser;

public sealed record SendEmailVerificationCodeCommand(string Email) : ICommand;

internal sealed class SendEmailVerificationCodeHandler
    : ICommandHandler<SendEmailVerificationCodeCommand>
{
    private readonly IEmailVerificationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public SendEmailVerificationCodeHandler(
        IEmailVerificationRepository repository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result> Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        // generate OTP
        var code = new Random().Next(100000, 999999).ToString();
        var expiresAt = DateTime.UtcNow.AddMinutes(5);

        // create entity (must include email now)
        var entity = EmailVerificationCode.Create(request.Email, code, expiresAt);

        // persist in DB
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // send via email service
        await _emailService.SendEmailAsync(
            request.Email,
            "Verify your email",
            $"<p>Your verification code is: <b>{code}</b>. It expires in 5 minutes.</p>"
        );

        return Result.Success();
    }
}

public sealed record SendEmailVerificationCodeRequest(string Email);