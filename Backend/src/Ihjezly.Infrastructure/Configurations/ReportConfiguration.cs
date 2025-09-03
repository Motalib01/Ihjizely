using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ihjezly.Infrastructure.Configurations;

internal sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason).HasMaxLength(1000);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

       
    }
}