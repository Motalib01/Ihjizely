using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Subscriptions;

public static class SubscriptionPlanErrors
{
    public static readonly Error NotFound = new(
        "SubscriptionPlan.NotFound",
        "The subscription plan with the specified identifier was not found.");

    public static readonly Error Inactive = new(
        "SubscriptionPlan.Inactive",
        "The selected subscription plan is inactive.");

    public static readonly Error InvalidDuration = new(
        "SubscriptionPlan.InvalidDuration",
        "The duration of the subscription plan is invalid.");

    public static readonly Error InvalidPrice = new(
        "SubscriptionPlan.InvalidPrice",
        "The price of the subscription plan is invalid.");
   
    public static Error MaxAdd = new(
        "SubscriptionPlan.MaxAdd",
        " You have reached the maximum number of ads allowed by your subscription.");
}