using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.Abstractions.Handlers;
public interface IHandlerCollection
{
    Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

