namespace Ihjezly.Infrastructure.Payments.Stripe;

public class StripeOptions
{
    public string SecretKey { get; set; } = default!;
    public string WebhookSecret { get; set; } = default!;
    public string SuccessUrl { get; set; } = default!;
    public string CancelUrl { get; set; } = default!;
}