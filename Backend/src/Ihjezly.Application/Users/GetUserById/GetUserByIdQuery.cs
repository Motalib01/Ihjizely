using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Users;

namespace Ihjezly.Application.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<User>;