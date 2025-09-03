using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Users.Common;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.GetAllUsers;

internal sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var userDtos = users.Select(u => u.ToDto()).ToList();
        return Result.Success(userDtos);
    }
}