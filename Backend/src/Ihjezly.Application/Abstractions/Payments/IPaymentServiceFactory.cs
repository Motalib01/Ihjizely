using Ihjezly.Domain.Shared;

namespace Ihjezly.Application.Abstractions.Payment;

public interface IPaymentServiceFactory
{
    IPaymentService GetService(PaymentMethod method);
}