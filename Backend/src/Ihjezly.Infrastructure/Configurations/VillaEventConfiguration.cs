using Ihjezly.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VillaEventConfiguration : IEntityTypeConfiguration<VillaEvent>
{
    public void Configure(EntityTypeBuilder<VillaEvent> builder)
    {
        builder.OwnsOne(x => x.Details, d =>
        {
            d.Property(p => p.NumberOfGuests);

            // Optional: configure Features as value collection if needed
            d.Property(p => p.Features)
                .HasConversion(
                    v => string.Join(',', v.Select(f => f.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => Enum.Parse<Features>(s)).ToList()
                );
        });

        builder.OwnsOne(x => x.Price, p =>
        {
            p.Property(m => m.Amount).HasColumnName("Price_Amount");
            p.Property(m => m.CurrencyCode).HasColumnName("Price_Currency");
        });
    }
}