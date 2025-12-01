

namespace EstudosIA.Version1.ApplicationCommon.Results;

public class ErrorInfo
{
    public string Code { get; set; }

    public string Message { get; set; }

    public Dictionary<string, object> Extensions { get; set; }

    public static ErrorInfo Create(string message, string code = null)
    {
        return new ErrorInfo { Code = code, Message = message };
    }




}
