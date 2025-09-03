using Ihjezly.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

public sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.OwnsOne(apartment => apartment.Details, details =>
        {
            details.Property(x => x.NumberOfAdults);
            details.Property(x => x.NumberOfChildren);
            details.Property(x => x.apartmentType).HasConversion<string>();
        });

        builder.OwnsOne(x => x.Price, p =>
        {
            p.Property(m => m.Amount).HasColumnName("Price_Amount");
            p.Property(m => m.CurrencyCode).HasColumnName("Price_Currency");
        });
    }
}