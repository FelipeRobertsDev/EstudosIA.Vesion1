using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.Abstractions.Handlers;

public class HandlerCollection : IHandlerCollection
{
    private readonly IServiceProvider _provider;

    public HandlerCollection(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = _provider.GetService(handlerType)
            ?? throw new InvalidOperationException($"Handler not found for {request.GetType().Name}");

        return await handler.HandleAsync((dynamic)request, cancellationToken);
    }
}
