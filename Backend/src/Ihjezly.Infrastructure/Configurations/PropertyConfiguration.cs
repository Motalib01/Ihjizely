using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired();
        builder.Property(p => p.Description).IsRequired();

        builder.OwnsOne(p => p.ViedeoUrl, v =>
        {
            v.Property(x => x.Url)
                .HasColumnName("ViedeoUrl")
                .IsRequired(false);
        });

        builder.OwnsOne(p => p.Location);

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("PropertyPrice_Amount");

            price.Property(p => p.CurrencyCode)
                .HasColumnName("PropertyPrice_Currency")
                .IsRequired()                        
                .HasMaxLength(3)
                .HasDefaultValue(Currency.Lyd.Code); 
        });

        builder.OwnsOne(p => p.Discount, discount =>
        {
            discount.Property(d => d.Value).HasColumnName("DiscountPercentage");
        });

        builder.Property(p => p.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.CreatedAt);
        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.IsAd).IsRequired();
        builder.Property(p => p.BusinessOwnerId).IsRequired();

        builder.HasOne<BusinessOwner>()
            .WithMany()
            .HasForeignKey(p => p.BusinessOwnerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<Apartment>(nameof(Apartment))
            .HasValue<Chalet>(nameof(Chalet))
            .HasValue<HotelRoom>(nameof(HotelRoom))
            .HasValue<HotelApartment>(nameof(HotelApartment))
            .HasValue<Resort>(nameof(Resort))
            .HasValue<RestHouse>(nameof(RestHouse))
            .HasValue<EventHallLarge>(nameof(EventHallLarge))
            .HasValue<EventHallSmall>(nameof(EventHallSmall))
            .HasValue<MeetingRoom>(nameof(MeetingRoom))
            .HasValue<VillaEvent>(nameof(VillaEvent));

        builder
            .Property(p => p.Facilities)
            .HasField("_facilities")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => string.Join(',', v.Select(f => f.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => Enum.Parse<Facility>(s)).ToList()
            );

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasDefaultValue(PropertyStatus.Pending);

        builder.OwnsMany(p => p.Images, img =>
        {
            img.WithOwner().HasForeignKey("PropertyId");
            img.HasKey("PropertyId", "Url");
            img.Property(p => p.Url).HasColumnName("ImageUrl");
            img.ToTable("PropertyImages");
        });

    }
}