using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Users.LogInUser;

public sealed record LoginUserCommand(string EmailOrPhone, string Password) : ICommand<AccessTokenResponse>;