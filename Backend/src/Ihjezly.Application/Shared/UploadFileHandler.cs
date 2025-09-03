using Ihjezly.Application.Abstractions.Files;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Ihjezly.Application.Shared;

internal sealed class UploadFileHandler : ICommandHandler<UploadFileCommand, List<string>>
{
    private readonly IFileService _fileService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadFileHandler(
        IFileService fileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _fileService = fileService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<List<string>>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var relativeUrls = await _fileService.UploadFilesAsync(request.Files, request.TargetFolder, cancellationToken);

        var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
        var absoluteUrls = relativeUrls.Select(url =>
            url.StartsWith("/") ? $"{baseUrl}{url}" : url).ToList();

        return Result.Success(absoluteUrls);
    }
}