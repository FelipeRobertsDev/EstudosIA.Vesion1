

namespace EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi
{
    public sealed class WhatsAppConnectionStateResult
    {
        public bool Success { get; init; }
        public string? InstanceName { get; init; }
        public string? State { get; init; }
        public bool Connected { get; init; }
        public string? Error { get; init; }
        public string? RawResponse { get; init; }

        public static WhatsAppConnectionStateResult Ok(
            string? instanceName,
            string? state,
            bool connected,
            string? rawResponse = null) =>
            new()
            {
                Success = true,
                InstanceName = instanceName,
                State = state,
                Connected = connected,
                RawResponse = rawResponse
            };

        public static WhatsAppConnectionStateResult Fail(
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
