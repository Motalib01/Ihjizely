using Ihjezly.Application.Abstractions.Files;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;
using Microsoft.AspNetCore.Http;

namespace Ihjezly.Application.Users.UpdateUser;

internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IFileService _fileService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IFileService fileService,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _fileService = fileService;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        string? imageUrl = null;

        if (request.ProfileImageFile is not null)
        {
            var urls = await _fileService.UploadFilesAsync(
                new List<IFormFile> { request.ProfileImageFile },
                "profile-images",
                cancellationToken
            );
            imageUrl = urls.FirstOrDefault();
        }

        var profilePicture = imageUrl is not null ? Image.Create(imageUrl) : user.UserProfilePicture;

        user.UpdateProfile(
            request.FullName,
            request.PhoneNumber,
            request.Email,
            profilePicture
        );

        _notificationRepository.Add(
            Notification.Create(user.Id, "Your profile information has been successfully updated.")
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}