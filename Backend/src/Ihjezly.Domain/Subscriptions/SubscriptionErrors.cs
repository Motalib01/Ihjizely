using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Domain.Subscriptions;

public static class SubscriptionErrors
{
    public static readonly Error SubscriptionNotFound = new(
        "Subscription.NotFound",
        "The subscription with the specified identifier was not found.");

    public static readonly Error PlanNotFound = new(
        "Subscription.PlanNotFound",
        "The subscription plan with the specified identifier was not found.");

    public static readonly Error ActiveSubscriptionNotFound = new(
        "Subscription.ActiveNotFound",
        "No active subscription found for the specified user.");
}