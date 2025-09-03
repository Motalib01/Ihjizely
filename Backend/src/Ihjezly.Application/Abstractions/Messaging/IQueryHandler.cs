using Ihjezly.Domain.Abstractions;
using MediatR;

namespace Ihjezly.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}