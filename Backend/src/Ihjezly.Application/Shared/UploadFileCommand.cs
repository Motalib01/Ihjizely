using Ihjezly.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Ihjezly.Application.Shared;
public sealed record UploadFileCommand(
    List<IFormFile> Files,
    string TargetFolder 
) : ICommand<List<string>>;