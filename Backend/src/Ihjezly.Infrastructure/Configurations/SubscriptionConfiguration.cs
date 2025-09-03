using Ihjezly.Domain.Subscriptions;
using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(x => x.Id);

        builder.HasOne<BusinessOwner>()
            .WithMany()
            .HasForeignKey(s => s.BusinessOwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<SubscriptionPlan>()
            .WithMany()
            .HasForeignKey(s => s.PlanId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(s => s.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("SubscriptionPrice_Amount")
                .HasColumnType("decimal(18,2)");

            price.Property(p => p.CurrencyCode)
                .HasColumnName("SubscriptionPrice_Currency")
                .HasMaxLength(3);
        });



        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();
    }
}