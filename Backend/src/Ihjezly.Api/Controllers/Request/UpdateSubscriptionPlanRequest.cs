namespace Ihjezly.Api.Controllers.Request;

public sealed class UpdateSubscriptionPlanRequest
{
    public Guid PlanId { get; set; }
    public string Name { get; set; } = string.Empty;

    // Store duration in days instead of TimeSpan
    public int DurationInDays { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int MaxAds { get; set; }
}