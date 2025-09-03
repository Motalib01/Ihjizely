using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Outbox;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(o => o.Content)
            .IsRequired();

        builder.Property(o => o.OccurredOnUtc)
            .IsRequired();

        builder.Property(o => o.ProcessedOnUtc);

        builder.Property(o => o.Error)
            .HasMaxLength(1000);
    }
}