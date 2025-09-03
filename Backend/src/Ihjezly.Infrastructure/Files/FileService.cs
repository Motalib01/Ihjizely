using Ihjezly.Application.Abstractions.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Ihjezly.Infrastructure.Files;

public sealed class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folder, CancellationToken cancellationToken = default)
    {
        var uploadPath = Path.Combine(_env.WebRootPath, "images", folder);
        Directory.CreateDirectory(uploadPath);

        var urls = new List<string>();

        foreach (var file in files)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            urls.Add($"/images/{folder}/{fileName}");
        }

        return urls;
    }

    public Task DeleteFilesAsync(List<string> fileUrls, CancellationToken cancellationToken = default)
    {
        foreach (var url in fileUrls)
        {
            var segments = url.Split('/');
            if (segments.Length < 3) continue;

            var folder = segments[2]; // e.g., "profiles" or "properties"
            var fileName = segments[^1];
            var fullPath = Path.Combine(_env.WebRootPath, "images", folder, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}