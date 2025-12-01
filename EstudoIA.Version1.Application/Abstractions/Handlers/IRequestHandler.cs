using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudosIA.Version1.ApplicationCommon.Results;
using System.Threading;

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
