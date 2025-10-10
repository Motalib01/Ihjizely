using Ihjezly.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[Owned]
public sealed record Money
{
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;

    [NotMapped]
    public Currency Currency => Currency.FromCode(CurrencyCode);

    private Money() { } // EF Core

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        Amount = amount;
        CurrencyCode = currency.Code;
    }

    public static Money Zero() => new(0, Currency.Lyd);

    public static Money operator +(Money first, Money second)
    {
        EnsureSameCurrency(first, second);
        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money operator -(Money first, Money second)
    {
        EnsureSameCurrency(first, second);
        return new Money(first.Amount - second.Amount, first.Currency);
    }

    public bool IsGreaterThanOrEqual(Money other)
    {
        EnsureSameCurrency(this, other);
        return Amount >= other.Amount;
    }

    public bool IsLessThan(Money other)
    {
        EnsureSameCurrency(this, other);
        return Amount < other.Amount;
    }

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.CurrencyCode != b.CurrencyCode)
            throw new InvalidOperationException($"Currency mismatch: {a.CurrencyCode} != {b.CurrencyCode}");
    }

    public override string ToString() => $"{Amount:0.##} {CurrencyCode}";
}