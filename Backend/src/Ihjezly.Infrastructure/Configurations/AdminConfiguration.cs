using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.Property(x => x.PhoneNumber).HasMaxLength(15);
        builder.HasIndex(x => x.PhoneNumber);
    }
}