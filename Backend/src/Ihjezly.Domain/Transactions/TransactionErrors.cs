using Ihjezly.Domain.Abstractions;

public static class TransactionErrors
{
    public static readonly Error NotFound = new(
        "Transaction.NotFound",
        "The transaction with the specified identifier was not found.");

    public static readonly Error InvalidAmount = new(
        "Transaction.InvalidAmount",
        "The transaction amount must be greater than zero.");

    public static readonly Error WalletNotFound = new(
        "Transaction.WalletNotFound",
        "The wallet associated with this transaction could not be found.");

    public static readonly Error InsufficientBalance = new(
        "Transaction.InsufficientBalance",
        "The wallet does not have sufficient balance for this transaction.");

    public static Error DoPTransFaild = new (
        "Transaction.DoPTransFaild",
        "Adfali DoPTrans failed.");

    public static Error ConfirmationFailed = new (
        "Transaction.ConfirmationFailed",
        "Edfali confirmation failed.");

    public static Error InitiationFailed = new(
        "Transaction.InitiationFailed",
        "Edfali transfer failed or returned empty transfer ID.");

    public static Error EdfaliTransferFailed = new(
        "Transaction.EdfaliTransferFailed",
        "Edfali transfer failed ");
}