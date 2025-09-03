using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Users.Common;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<CurrentUserDto>(UserErrors.NotFound);

        var dto = new CurrentUserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Role.ToString(),
            user.IsVerified,
            user.UserProfilePicture?.Url
        );

        return Result.Success(dto);
    }
}