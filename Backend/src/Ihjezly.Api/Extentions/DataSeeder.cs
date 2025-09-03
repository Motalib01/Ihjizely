using Ihjezly.Infrastructure;

namespace Ihjezly.Api.Extentions;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.SubscriptionPlans.Any())
        {
            //context.SubscriptionPlans.Add(new SubscriptionPlan("Basic", 100, TimeSpan.FromDays(30)));
            //context.SubscriptionPlans.Add(new SubscriptionPlan("Premium", 300, TimeSpan.FromDays(90), false));
            //await context.SaveChangesAsync();
        }

        // Add more seed data here
    }
}
