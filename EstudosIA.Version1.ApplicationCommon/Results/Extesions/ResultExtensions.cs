using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace EstudosIA.Version1.ApplicationCommon.Results.Extentions
{
    public static class ResultExtensions
    {
        public class ProblemDetails
        {
            [JsonPropertyOrder(-5)]
            [JsonPropertyName("type")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string Type { get; set; }

            [JsonPropertyOrder(-4)]
            [JsonPropertyName("title")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string Title { get; set; }

            [JsonPropertyOrder(-3)]
            [JsonPropertyName("status")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public int Status { get; set; }

            [JsonPropertyOrder(-2)]
            [JsonPropertyName("detail")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string Detail { get; set; }

            [JsonPropertyOrder(-1)]
            [JsonPropertyName("instance")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string Instance { get; set; }

            [JsonExtensionData]
            public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);
        }
        public static IActionResult AsActionResult<T>(this Result<T> result)
        {
            ArgumentNullException.ThrowIfNull(result, nameof(result));

            if (result.HasErrors)
            {
                return new ObjectResult(new ProblemDetails
                {
                    Status = (int)result.StatusCode,
                    Title = "One or more errors occurred",
                    Detail = string.Join("; ", result.Errors.Select(e => e.Message)),
                    Extensions = new Dictionary<string, object>
                    {
                        { "errors", result.Errors }
                    }
                })
                {
                    StatusCode = (int)result.StatusCode
                };
            }

            return new ObjectResult(result.Value)
            {
                StatusCode = (int)result.StatusCode
            };
        }
    }
}
