using Ihjezly.Domain.Transactions;
using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(x => x.PhoneNumber).HasMaxLength(15);
        builder.HasIndex(x => x.PhoneNumber);
    }
}

