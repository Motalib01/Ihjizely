using Ihjezly.Domain.Shared;

namespace Ihjezly.Api.Controllers.Request;

public sealed class AddFundsTransactionRequest
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = default!;
    public string Description { get; init; } = default!;
    public PaymentMethod PaymentMethod { get; init; }
}