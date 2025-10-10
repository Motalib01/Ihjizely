using Ihjezly.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(255);

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("PlanPrice_Amount")
                .HasPrecision(18, 2);

            price.Property(p => p.CurrencyCode)
                .HasColumnName("PlanPrice_Currency")
                .HasMaxLength(3);

            price.WithOwner().HasForeignKey("PlanId");
        });

    }
}