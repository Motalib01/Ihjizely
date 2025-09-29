using Ihjezly.Application.Abstractions.Payment;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Ihjezly.Infrastructure.Payments.Stripe;

public class StripePaymentService : IPaymentService
{
    private readonly StripeOptions _options;

    public StripePaymentService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        StripeConfiguration.ApiKey = _options.SecretKey; // Requires 'using Stripe;'
    }

    public async Task<(bool IsSuccess, string? TransactionId, string? Error)> StartPaymentAsync(
        string payerIdentifier,
        decimal amount,
        string currency,
        string description)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(amount * 100),
                            Currency = currency.ToLower(),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = description
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = _options.SuccessUrl,
                CancelUrl = _options.CancelUrl
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return (true, session.Id, null);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, string? Result)> ConfirmPaymentAsync(string transactionId)
    {
        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(transactionId);

            return session.PaymentStatus == "paid"
                ? (true, "OK")
                : (false, session.PaymentStatus);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}