using Ihjezly.Application.Abstractions.Payment;

namespace Ihjezly.Infrastructure.Payments.Paypal;

public class PayPalPaymentService : IPaymentService
{
    public async Task<(bool IsSuccess, string? TransactionId, string? Error)> StartPaymentAsync(
        string payerIdentifier,
        decimal amount,
        string currency,
        string description)
    {
        // Call PayPal API using description
        return (true, "paypal-session-id", null);
    }

    public async Task<(bool IsSuccess, string? Result)> ConfirmPaymentAsync(string transactionId)
    {
        return (true, "OK");
    }
}