namespace Ihjezly.Infrastructure.Payments.Masarat;

public class MasaratOptions
{
    public string BaseUrl { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string Pin { get; set; } = default!;
    public string ProviderId { get; set; } = default!;
    public int AuthUserType { get; set; }
}