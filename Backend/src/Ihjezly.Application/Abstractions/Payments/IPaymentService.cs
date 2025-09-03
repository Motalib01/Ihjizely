namespace Ihjezly.Application.Abstractions.Payment;

public interface IPaymentService
{
    Task<(bool IsSuccess, string? TransactionId, string? Error)> StartPaymentAsync(string customerId, decimal amount, string currency, string description);
    Task<(bool IsSuccess, string? Result)> ConfirmPaymentAsync(string transactionId);
}