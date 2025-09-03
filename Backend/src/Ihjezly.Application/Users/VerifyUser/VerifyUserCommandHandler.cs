using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.VerifyUser;

internal sealed class VerifyUserCommandHandler : ICommandHandler<VerifyUserCommand>
{
    private readonly IClientRepository _clientRepository;
    private readonly IBusinessOwnerRepository _businessOwnerRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyUserCommandHandler(
        IClientRepository clientRepository,
        IBusinessOwnerRepository businessOwnerRepository,
        IAdminRepository adminRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _businessOwnerRepository = businessOwnerRepository;
        _adminRepository = adminRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (client is not null)
        {
            client.MarkPhoneAsVerified();
            _notificationRepository.Add(Notification.Create(client.Id, "تم التحقق من رقم هاتفك بنجاح."));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        var businessOwner = await _businessOwnerRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (businessOwner is not null)
        {
            businessOwner.MarkPhoneAsVerified();
            _notificationRepository.Add(Notification.Create(businessOwner.Id, "تم التحقق من رقم هاتفك بنجاح."));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        var admin = await _adminRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (admin is not null)
        {
            admin.MarkPhoneAsVerified();
            _notificationRepository.Add(Notification.Create(admin.Id, "تم التحقق من رقم هاتفك بنجاح."));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        return Result.Failure(UserErrors.NotFound);
    }
}