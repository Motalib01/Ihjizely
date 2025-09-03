using Ihjezly.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.CurrencyCode)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        builder.HasOne<Wallet>()
            .WithMany()
            .HasForeignKey(t => t.WalletId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}