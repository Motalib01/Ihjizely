using Ihjezly.Application.Abstractions.Files;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Shared;

internal sealed class DeleteFileHandler : ICommandHandler<DeleteFileCommand>
{
    private readonly IFileService _fileService;

    public DeleteFileHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        await _fileService.DeleteFilesAsync(request.FileUrls, cancellationToken);
        return Result.Success();
    }
}