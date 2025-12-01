using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.Abstractions.Handlers;

public interface IHandler<in TRequest, TResponse> : IHandler where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> HandlerAsync(TRequest request, CancellationToken cancellationToken = default);
}
