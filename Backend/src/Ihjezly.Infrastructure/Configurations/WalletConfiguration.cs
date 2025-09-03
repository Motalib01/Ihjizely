using Ihjezly.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Wallet>(w => w.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(w => w.Balance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Balance_Amount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.CurrencyCode)
                .HasColumnName("Balance_Currency")
                .HasMaxLength(3);
        });
    }
}