using Ihjezly.Application.Abstractions.Messaging;

namespace Ihjezly.Application.Shared;

public sealed record DeleteFileCommand(
    List<string> FileUrls
) : ICommand;