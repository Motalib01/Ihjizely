using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Users.Common;

namespace Ihjezly.Application.Users.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IQuery<CurrentUserDto>;