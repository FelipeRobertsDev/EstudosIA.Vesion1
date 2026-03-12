using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi
{
    public sealed class WhatsAppInstanceCreateResult
    {
        public bool Success { get; init; }
        public string? InstanceName { get; init; }
        public string? Error { get; init; }
        public string? RawResponse { get; init; }

        public static WhatsAppInstanceCreateResult Ok(
            string? instanceName,
            string? rawResponse = null) =>
            new()
            {
                Success = true,
                InstanceName = instanceName,
                RawResponse = rawResponse
            };

        public static WhatsAppInstanceCreateResult Fail(
            string error,
            string? rawResponse = null) =>
            new()
            {
                Success = false,
                Error = error,
                RawResponse = rawResponse
            };
    }
}
