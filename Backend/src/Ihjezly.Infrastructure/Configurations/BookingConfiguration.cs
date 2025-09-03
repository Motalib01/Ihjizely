using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.TotalPrice, p => p.Property(x => x.Amount).HasColumnType("decimal(18,2)"));

        builder.HasOne<Client>()
            .WithMany()
            .HasForeignKey(b => b.ClientId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Property>()
            .WithMany()
            .HasForeignKey(b => b.PropertyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(b => b.TotalPrice, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("TotalPrice_Amount")
                .HasColumnType("decimal(18,2)");

            price.Property(p => p.CurrencyCode)
                .HasColumnName("TotalPrice_Currency")
                .HasMaxLength(3);

            price.WithOwner().HasForeignKey("BookingId");
        });




    }
}