using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Users.VerifyUser;

public sealed record VerifyUserCommand(string PhoneNumber) : ICommand;