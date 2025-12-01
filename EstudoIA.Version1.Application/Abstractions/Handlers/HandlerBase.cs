using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.Abstractions.Handlers;

public abstract class HandlerBase<TRequest, TResponse>
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return Result<TResponse>.Failure(validation.Errors);

        return await ExecuteAsync(request, cancellationToken);
    }

    protected abstract Task<ValidationResult> ValidateAsync(
        TRequest request,
        CancellationToken cancellationToken);

    protected abstract Task<Result<TResponse>> ExecuteAsync(
        TRequest request,
        CancellationToken cancellationToken);
}

