using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Message).HasMaxLength(1000);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}