namespace Ihjezly.Api.Controllers.Request;

public sealed record DeductFundsTransactionRequest(
    decimal Amount,
    string Currency,
    string Description);