namespace Ihjezly.Application.Transactions.AdminAddFunds;

public sealed class AdminAddFundsRequest
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = default!;
    public string Description { get; init; } = default!;
}