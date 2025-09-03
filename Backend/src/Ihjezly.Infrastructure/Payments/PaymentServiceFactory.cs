using Ihjezly.Application.Abstractions.Payment;
using Ihjezly.Domain.Shared;
using Ihjezly.Infrastructure.Payments.Masarat;
using Ihjezly.Infrastructure.Payments.Paypal;
using Ihjezly.Infrastructure.Payments.Stripe;
using Ihjezly.Infrastructure.Payments.Tdsp;
using Microsoft.Extensions.DependencyInjection;

namespace Ihjezly.Infrastructure.Payments;

public sealed class PaymentServiceFactory : IPaymentServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentService GetService(PaymentMethod method) =>
        method switch
        {
            PaymentMethod.PayPal => _serviceProvider.GetRequiredService<PayPalPaymentService>(),
            PaymentMethod.Stripe => _serviceProvider.GetRequiredService<StripePaymentService>(),
            PaymentMethod.Masarat => _serviceProvider.GetRequiredService<MasaratPaymentService>(),
            PaymentMethod.Tdsp => _serviceProvider.GetRequiredService<TdspPaymentService>(),
            _ => throw new NotImplementedException($"Payment method {method} is not supported")
        };
}
