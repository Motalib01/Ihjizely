namespace Ihjezly.Application.Payments.Tdsp;

public sealed class TdspOptions
{
    public string BaseUrl { get; set; } = string.Empty; // e.g. https://c7drkx2ege.execute-api.eu-west-2.amazonaws.com/
    public string StoreId { get; set; } = string.Empty;
    public string BearerToken { get; set; } = string.Empty;
    public string BackendUrl { get; set; } = string.Empty;
    public string FrontendUrl { get; set; } = string.Empty;
}
