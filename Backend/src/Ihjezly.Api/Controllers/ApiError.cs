using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Api.Controllers;

public sealed class ApiError
{
    public string Code { get; init; }
    public string Message { get; init; }

    public ApiError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static ApiError From(Error error) => new(error.Code, error.Message);
}