namespace Ihjezly.Domain.Shared;

public sealed record Currency(string Code)
{
    public static readonly Currency None = new("");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    public static readonly Currency Lyd = new("LYD");

    public static readonly IReadOnlyCollection<Currency> All = new[] { Usd, Eur, Lyd };

    public static Currency FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Currency code cannot be null or empty.");

        return All.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException($"Currency '{code}' is invalid.");
    }

    public static bool IsValidCode(string code) =>
        !string.IsNullOrWhiteSpace(code) &&
        All.Any(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    public override string ToString() => Code;
}