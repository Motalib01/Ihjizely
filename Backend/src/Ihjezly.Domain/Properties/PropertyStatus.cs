namespace Ihjezly.Domain.Properties;

public enum PropertyStatus
{
    Pending,
    Accepted,
    Refused
}

public static class PropertyStatusExtensions
{
    public static string ToArabic(this PropertyStatus status)
    {
        return status switch
        {
            PropertyStatus.Pending => "قيد الانتظار",
            PropertyStatus.Accepted => "قبول",
            PropertyStatus.Refused => "رفض",
            _ => status.ToString()
        };
    }
}