using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Ihjezly.Infrastructure.Payments.Stripe;

namespace Ihjezly.Api.Controllers;

[Route("api/webhooks/stripe")]
[ApiController]
public class StripeWebhookController : ControllerBase
{
    private readonly StripeOptions _options;

    public StripeWebhookController(IOptions<StripeOptions> options)
    {
        _options = options.Value;
    }

    [HttpPost]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent;

        try
        {
            var signatureHeader = Request.Headers["Stripe-Signature"];
            stripeEvent = EventUtility.ConstructEvent(
                json,
                signatureHeader,
                _options.WebhookSecret
            );
        }
        catch (StripeException e)
        {
            return BadRequest($"⚠️ Webhook error: {e.Message}");
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            var sessionId = session?.Id;
            var customerEmail = session?.CustomerDetails?.Email;
            var amountTotal = session?.AmountTotal;

            // TODO: match sessionId to transaction and confirm it
        }

        return Ok();
    }

}