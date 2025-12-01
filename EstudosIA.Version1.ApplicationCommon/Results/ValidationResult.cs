using EstudosIA.Version1.ApplicationCommon.Results;

public sealed class ValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<ErrorInfo> Errors { get; } = new();

    public static ValidationResult Success => new();

    public static ValidationResult Failure(params ErrorInfo[] errors)
    {
        var result = new ValidationResult();
        result.Errors.AddRange(errors);
        return result;
    }
}
