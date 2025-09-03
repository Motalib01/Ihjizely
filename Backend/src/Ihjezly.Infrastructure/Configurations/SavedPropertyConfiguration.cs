using Ihjezly.Domain.Properties;
using Ihjezly.Domain.SavedProperties;
using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class SavedPropertyConfiguration : IEntityTypeConfiguration<SavedProperty>
{
    public void Configure(EntityTypeBuilder<SavedProperty> builder)
    {
        builder.ToTable("SavedProperties");
        builder.HasKey(x => x.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(sp => sp.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Property>()
            .WithMany()
            .HasForeignKey(sp => sp.PropertyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}