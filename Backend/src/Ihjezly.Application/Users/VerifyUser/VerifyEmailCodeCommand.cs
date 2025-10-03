using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.VerifyUser;

public sealed record VerifyEmailCodeCommand(string Mail, string Code) : ICommand<bool>;

internal sealed class VerifyEmailCodeHandler
    : ICommandHandler<VerifyEmailCodeCommand, bool>
{
    private readonly IEmailVerificationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailCodeHandler(
        IEmailVerificationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(VerifyEmailCodeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetActiveCodeAsync(request.Mail, cancellationToken);

        if (entity == null)
            return Result.Failure<bool>(VerifyEmailCodeError.Invalid);

        if (!entity.IsValid())
            return Result.Failure<bool>(VerifyEmailCodeError.Invalid);

        entity.MarkAsUsed();
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}

public sealed record VerifyEmailCodeRequest(string Mail, string Code);