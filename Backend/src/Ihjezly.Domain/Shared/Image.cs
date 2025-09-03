using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Domain.Shared;
[Owned]
public sealed record Image(string Url)
{
    public static readonly Image Default = new("http://ihjezly.runasp.net/images/ProfailPicture.jpg");

    public static Image Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("Image URL cannot be null or empty.");

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) throw new ArgumentException("Invalid image URL.");

        return new Image(url);
    }
}