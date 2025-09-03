using Ihjezly.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class HotelRoomConfiguration : IEntityTypeConfiguration<HotelRoom>
{
    public void Configure(EntityTypeBuilder<HotelRoom> builder)
    {
        builder.OwnsOne(x => x.Details, d =>
        {
            d.Property(p => p.NumberOfAdults);
            d.Property(p => p.NumberOfChildren);
            d.Property(p => p.hotelRoomType)
                .HasConversion<string>()
                .HasColumnName("RoomType");

            d.Property(p => p.clasification)
                .HasConversion<string>()
                .HasColumnName("Clasification");
        });

        builder.OwnsOne(x => x.Price, p =>
        {
            p.Property(m => m.Amount).HasColumnName("Price_Amount");
            p.Property(m => m.CurrencyCode).HasColumnName("Price_Currency");
        });
    }
}