using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Discriminator configuration for inheritance (TPH)
        builder.HasDiscriminator<UserRole>("Role")
            .HasValue<Admin>(UserRole.Admin)
            .HasValue<Client>(UserRole.Client)
            .HasValue<BusinessOwner>(UserRole.BusinessOwner);

        // Primary Key
        builder.HasKey(u => u.Id);

        // Basic properties
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PhoneNumber)
            .IsRequired(false)
            .HasMaxLength(15);

        builder.Property(u => u.Email)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique();

        builder.Property(u => u.Password)
            .IsRequired();

        builder.Property(u => u.IsVerified)
            .IsRequired();

        // Role (Enum)
        builder.Property(u => u.Role)
            .HasConversion<string>() // Optional: store enum as string
            .IsRequired();

        // UserProfilePicture (Image value object)
        builder.OwnsOne(u => u.UserProfilePicture, pic =>
        {
            pic.Property(p => p.Url)
                .HasColumnName("UserProfilePicture_Url")
                .HasMaxLength(255);
        });
    }
}