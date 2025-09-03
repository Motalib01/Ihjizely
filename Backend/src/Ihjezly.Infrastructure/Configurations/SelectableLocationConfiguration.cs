using Ihjezly.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

public class SelectableLocationConfiguration : IEntityTypeConfiguration<SelectableLocation>
{
    public void Configure(EntityTypeBuilder<SelectableLocation> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.City).IsRequired();
        builder.Property(l => l.State).IsRequired();
        builder.Property(l => l.Country).IsRequired();
        builder.ToTable("SelectableLocations");
    }
}