using Ihjezly.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

public sealed class EventHallLargeConfiguration : IEntityTypeConfiguration<EventHallLarge>
{
    public void Configure(EntityTypeBuilder<EventHallLarge> builder)
    {
        builder.OwnsOne(x => x.Details, d =>
        {
            d.Property(p => p.NumberOfGuests);

            d.Property(p => p.Features)
                .HasConversion(
                    v => string.Join(',', v), 
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => Enum.Parse<Features>(x))
                        .ToList()             
                )
                .HasColumnName("Features")
                .HasMaxLength(200);
        });
        builder.OwnsOne(x => x.Price, p =>
        {
            p.Property(m => m.Amount).HasColumnName("Price_Amount");
            p.Property(m => m.CurrencyCode).HasColumnName("Price_Currency");
        });
    }
}