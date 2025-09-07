using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.CheckPhoneNumber;

public sealed record CheckPhoneNumberQuery(string PhoneNumber) : IQuery<bool>;

internal sealed class CheckPhoneNumberQueryHandler
    : IQueryHandler<CheckPhoneNumberQuery, bool>
{
    private readonly IUserRepository _userRepository;

    public CheckPhoneNumberQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(CheckPhoneNumberQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        return user is not null;
    }
}