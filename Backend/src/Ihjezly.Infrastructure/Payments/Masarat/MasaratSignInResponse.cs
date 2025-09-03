namespace Ihjezly.Infrastructure.Payments.Masarat;

public class MasaratSignInResponse
{
    public int Type { get; set; }
    public List<string>? Messages { get; set; }
    public string? TraceId { get; set; }
    public MasaratSignInContent? Content { get; set; }
}