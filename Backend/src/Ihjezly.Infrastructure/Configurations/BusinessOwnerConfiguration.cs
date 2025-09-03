using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class BusinessOwnerConfiguration : IEntityTypeConfiguration<BusinessOwner>
{
    public void Configure(EntityTypeBuilder<BusinessOwner> builder)
    {
        builder.Property(x => x.PhoneNumber).HasMaxLength(15);
        builder.HasIndex(x => x.PhoneNumber);
    }
}