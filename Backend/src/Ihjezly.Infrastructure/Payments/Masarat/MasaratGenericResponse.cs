namespace Ihjezly.Infrastructure.Payments.Masarat;

public class MasaratGenericResponse
{
    public int Type { get; set; }
    public List<string>? Messages { get; set; }
    public string? TraceId { get; set; }
    public string? Content { get; set; }
}