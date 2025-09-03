namespace Ihjezly.Application.Payments.Masarat;

public static class MasaratOtpProvider
{
    private static string? _otp;

    public static void SetOtp(string otp) => _otp = otp;
    public static string GetOtp() => _otp ?? throw new InvalidOperationException("OTP not set");
}