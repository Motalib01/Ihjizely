using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.CheckPhoneNumber;

public sealed record CheckPhoneNumberQuery(string PhoneNumber) : IQuery<CheckPhoneNumberResponse>;


internal sealed class CheckPhoneNumberQueryHandler
    : IQueryHandler<CheckPhoneNumberQuery, CheckPhoneNumberResponse>
{
    private readonly IUserRepository _userRepository;

    public CheckPhoneNumberQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<CheckPhoneNumberResponse>> Handle(CheckPhoneNumberQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

        if (user is null)
        {
            return new CheckPhoneNumberResponse(false, null);
        }

        return new CheckPhoneNumberResponse(true, user.Role.ToString());
    }
}

public sealed record CheckPhoneNumberResponse(
    bool Exists,
    string? Role
);