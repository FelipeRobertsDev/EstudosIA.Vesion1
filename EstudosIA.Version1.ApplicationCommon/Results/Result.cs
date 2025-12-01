using System.Net;

namespace EstudosIA.Version1.ApplicationCommon.Results;

public class Result<T>
{
    private readonly List<ErrorInfo> _errors = new List<ErrorInfo>();

    private readonly T _value;

    public T Value => _value;

    public HttpStatusCode StatusCode { get; }

    public bool HasValue => _value != null;

    public bool HasErrors => _errors.Count != 0;

    public IReadOnlyCollection<ErrorInfo> Errors => _errors.AsReadOnly();

    protected Result(HttpStatusCode statusCode, T value, IEnumerable<ErrorInfo> errors = null)
    {
        StatusCode = statusCode;
        _value = value;
        if (errors != null)
        {
            _errors.AddRange(errors);
        }
    }

    public static Result<T> Success(T value, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        if ((int)statusCode >= 400)
        {
            throw new ArgumentException("Status code must indicate success (less than 400).", nameof(statusCode));
        }

        return new Result<T>(statusCode, value);
    }

    public static Result<T> Success(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return Success(default(T), statusCode);
    }

    public static Result<T> Failure(IEnumerable<ErrorInfo> errors, T value = default, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        if (errors == null || !errors.Any())
            throw new ArgumentException("Errors must be provided for a failure result.", nameof(errors));

        return new Result<T>(statusCode, value, errors);
    }

    public static Result<T> Failure(ErrorInfo error, T value = default, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));

        return new Result<T>(statusCode, value, new List<ErrorInfo> { error });
    }

 
    public Result<TOut> Map<TOut>(Func<T, TOut> mapFunction)
    {
        if (mapFunction == null)
            throw new ArgumentNullException(nameof(mapFunction));

        return HasErrors
            ? Result<TOut>.Failure(_errors, default, StatusCode)
            : Result<TOut>.Success(mapFunction(_value), StatusCode);
    }
}
