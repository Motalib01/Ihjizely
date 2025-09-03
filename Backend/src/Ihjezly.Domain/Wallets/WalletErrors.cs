using Ihjezly.Domain.Abstractions;

public static class WalletErrors
{
    public static readonly Error NotFound = new(
        "Wallet.NotFound",
        "The wallet with the specified identifier was not found.");

    public static readonly Error CurrencyMismatch = new(
        "Wallet.CurrencyMismatch",
        "The currency of the wallet does not match the currency of the requested transaction.");

    public static readonly Error AlreadyExists = new(
        "Wallet.AlreadyExists",
        "A wallet for the specified user already exists.");

    public static readonly Error InvalidAmount = new(
        "Wallet.InvalidAmount",
        "The specified amount must be greater than zero.");

    public static Error InsufficientBalance = new(
        "Wallet.InsufficientBalance",
        "The wallet does not have sufficient balance for this operation.");
}