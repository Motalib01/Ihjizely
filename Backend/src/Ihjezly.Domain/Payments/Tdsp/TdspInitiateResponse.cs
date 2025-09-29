namespace Ihjezly.Infrastructure.Payments.Tdsp;

public sealed class TdspInitiateResponse
{
    public string result { get; set; } = string.Empty;
    public string custom_ref { get; set; } = string.Empty;
    public string url { get; set; } = string.Empty;
}