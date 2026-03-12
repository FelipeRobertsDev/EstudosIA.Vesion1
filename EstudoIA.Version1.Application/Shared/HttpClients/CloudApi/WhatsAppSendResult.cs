using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Shared.HttpClients.CloudApi
{
    public sealed class WhatsAppSendResult
    {
        public bool Success { get; init; }
        public string? MessageId { get; init; }
        public string? RawResponse { get; init; }
        public string? Error { get; init; }

        public static WhatsAppSendResult Ok(string? messageId, string? rawResponse) =>
            new()
            {
                Success = true,
                MessageId = messageId,
                RawResponse = rawResponse
            };

        public static WhatsAppSendResult Fail(string? error, string? rawResponse = null) =>
            new()
            {
                Success = false,
                Error = error,
                RawResponse = rawResponse
            };
    }
}
