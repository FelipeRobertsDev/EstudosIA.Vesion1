using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago.ResponseJsonWrapper;

public class PaymentGatewayMercadoPagoHttpClient
    : IPaymentGatewayAbacatePayHttpClient
{
    private static readonly string _pathBillingCreate = "billing/create";
    private readonly HttpClient _httpClient;

    public PaymentGatewayMercadoPagoHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentResponse> ChargePaymentAsync(
        PaymentGatewayMercadoPagoRequest request,
        CancellationToken cancellationToken)
    {
        
        var methods = request.Methods.Select(m => m switch
        {
            PaymentMethod.Pix => "PIX",
            PaymentMethod.CreditCard => "CARD",
            PaymentMethod.Boleto => "BOLETO",
            _ => throw new ArgumentOutOfRangeException(nameof(m))
        }).ToList();


        var payload = new
        {
            frequency = "ONE_TIME",
            externalId = request.ExternalId,
            methods,
            products = request.Products.Select(p => new
            {
                externalId = p.ExternalId,
                name = p.Name,
                description = p.Description,
                quantity = p.Quantity,
                price = p.PriceInCents
            }),
            customer = new
            {
                name = request.Customer.Name,
                email = request.Customer.Email,
                cellphone = request.Customer.Cellphone,
                taxId = request.Customer.TaxId
            },
            returnUrl = request.ReturnUrl,
            completionUrl = request.CompletionUrl
        };



        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        // 4️⃣ Chamada HTTP
        var response = await _httpClient.PostAsync(
            _pathBillingCreate,
            content,
            cancellationToken);

        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Erro ao criar cobrança AbacatePay: {(int)response.StatusCode} - {error}");
        }

        
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var wrapper = JsonSerializer.Deserialize<
        AbacatePayResponseWrapper<AbacatePayBillingCreateResponse>
            >(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

        if (wrapper?.Data == null)
            throw new InvalidOperationException("Resposta inválida da AbacatePay.");

        var abacateResponse = wrapper.Data;



        return new PaymentResponse
        {
            PaymentId = abacateResponse.Id,
            ExternalId = abacateResponse.ExternalId,
            Status = MapStatus(abacateResponse.Status),
            AmountInCents = abacateResponse.Amount,
            CheckoutUrl = abacateResponse.Url,
            Methods = MapMethodsBack(abacateResponse.Methods),
            CreatedAt = abacateResponse.CreatedAt
        };


    }



    private static List<PaymentMethod> MapMethodsBack(IEnumerable<string> methods)
        => methods.Select(m => m switch
        {
            "PIX" => PaymentMethod.Pix,
            "CARD" => PaymentMethod.CreditCard,
            "BOLETO" => PaymentMethod.Boleto,
            _ => throw new ArgumentOutOfRangeException(nameof(m))
        }).ToList();

    private static PaymentStatus MapStatus(string status)
        => status switch
        {
            "PENDING" => PaymentStatus.Pending,
            "PAID" => PaymentStatus.Paid,
            "CANCELLED" => PaymentStatus.Cancelled,
            "EXPIRED" => PaymentStatus.Expired,
            _ => PaymentStatus.Failed
        };
}

internal sealed class AbacatePayBillingCreateResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    [JsonPropertyName("methods")]
    public List<string> Methods { get; set; } = new();

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("externalId")]
    public string ExternalId { get; set; } = null!;
}

