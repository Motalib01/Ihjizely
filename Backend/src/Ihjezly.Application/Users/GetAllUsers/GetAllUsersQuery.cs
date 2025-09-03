using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Users.Common;

namespace Ihjezly.Application.Users.GetAllUsers;

public sealed record GetAllUsersQuery() : IQuery<List<UserDto>>;