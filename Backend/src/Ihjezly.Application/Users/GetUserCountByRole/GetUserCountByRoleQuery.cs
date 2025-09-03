using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Users;

namespace Ihjezly.Application.Users.GetUserCountByRole;

public sealed record GetUserCountByRoleQuery(UserRole Role) : IQuery<int>;