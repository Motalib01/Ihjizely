using Ihjezly.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

public sealed class ChaletConfiguration : IEntityTypeConfiguration<Chalet>
{
    public void Configure(EntityTypeBuilder<Chalet> builder)
    {
        builder.OwnsOne(x => x.Details, d =>
        {
            d.Property(p => p.NumberOfAdults);
            d.Property(p => p.NumberOfChildren);
        });

        builder.OwnsOne(x => x.Price, p =>
        {
            p.Property(m => m.Amount).HasColumnName("Price_Amount");
            p.Property(m => m.CurrencyCode).HasColumnName("Price_Currency");
        });
    }
}