

using System.Text.Json.Serialization;

namespace EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago.ResponseJsonWrapper;

internal sealed class AbacatePayResponseWrapper<T>
{
    [JsonPropertyName("error")]
    public object? Error { get; set; }

    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;

}
