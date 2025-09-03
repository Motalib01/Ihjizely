using Ihjezly.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ResortConfiguration : IEntityTypeConfiguration<Resort>
{
    public void Configure(EntityTypeBuilder<Resort> builder)
    {
        builder.OwnsOne(x => x.Details, d =>
        {
            d.Property(p => p.NumberOfAdults);
            d.Property(p => p.NumberOfChildren);
            d.Property(p => p.type)
                .HasConversion<string>()
                .HasMaxLength(50);

            d.Property(p => p.clasification)
                .HasConversion<string>()
                .HasMaxLength(50);
        });


        builder.OwnsOne(x => x.Price, p =>
        {
            p.Property(m => m.Amount).HasColumnName("Price_Amount");
            p.Property(m => m.CurrencyCode).HasColumnName("Price_Currency");
        });
    }
}