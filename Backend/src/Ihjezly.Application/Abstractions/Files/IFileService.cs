using Microsoft.AspNetCore.Http;

namespace Ihjezly.Application.Abstractions.Files;

public interface IFileService
{
    Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folder, CancellationToken cancellationToken = default);
    Task DeleteFilesAsync(List<string> fileUrls, CancellationToken cancellationToken = default);
}