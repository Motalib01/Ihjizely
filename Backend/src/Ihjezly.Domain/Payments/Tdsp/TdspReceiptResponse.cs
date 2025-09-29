namespace Ihjezly.Infrastructure.Payments.Tdsp;

public sealed class TdspReceiptResponse
{
    public string result { get; set; } = string.Empty;
    public TdspReceiptData? data { get; set; }
}