using Ihjezly.Domain.Abstractions;
using MediatR;

namespace Ihjezly.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{

}