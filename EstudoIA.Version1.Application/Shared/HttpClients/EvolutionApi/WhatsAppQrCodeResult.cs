using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi
{
    public sealed class WhatsAppQrCodeResult
    {
        public bool Success { get; init; }
        public string? InstanceName { get; init; }
        public string? Base64 { get; init; }
        public string? Code { get; init; }
        public string? Error { get; init; }
        public string? RawResponse { get; init; }

        public static WhatsAppQrCodeResult Ok(
            string? instanceName,
            string? base64,
            string? code,
            string? rawResponse = null) =>
            new()
            {
                Success = true,
                InstanceName = instanceName,
                Base64 = base64,
                Code = code,
                RawResponse = rawResponse
            };

        public static WhatsAppQrCodeResult Fail(
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
