using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.GetUserCountByRole;

internal sealed class GetUserCountByRoleQueryHandler : IQueryHandler<GetUserCountByRoleQuery, int>
{
    private readonly IUserRepository _userRepository;

    public GetUserCountByRoleQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<int>> Handle(GetUserCountByRoleQuery request, CancellationToken cancellationToken)
    {
        var count = await _userRepository.CountByRoleAsync(request.Role, cancellationToken);
        return Result.Success(count);
    }
}