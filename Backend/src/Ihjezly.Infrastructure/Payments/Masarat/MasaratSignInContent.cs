namespace Ihjezly.Infrastructure.Payments.Masarat;

public class MasaratSignInContent
{
    public string? ValidTo { get; set; }
    public string? RefreshToken { get; set; }
    public string? SystemIdentity { get; set; }
    public int Creds { get; set; }
    public int Tag { get; set; }
    public string? Value { get; set; }
}