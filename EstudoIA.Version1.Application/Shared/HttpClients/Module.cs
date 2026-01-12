using EstudoIA.Version1.Application.Shared.HttpClients.IA;
using EstudoIA.Version1.Application.Shared.HttpClients.IA.Gemini;
using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;
using EstudoIA.Version1.Application.Shared.Places;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class HttpClientExtensions
{
    public static IServiceCollection AddHttpClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===== MERCADO PAGO (já existente) =====
        var mercadoPagoSettings = configuration.GetSection("Payment:AbacatePay");
        var accessToken = mercadoPagoSettings["ChaveSecret"];
        var baseUrl = mercadoPagoSettings["BaseUrl"];

        services.AddHttpClient<IPaymentGatewayAbacatePayHttpClient, PaymentGatewayMercadoPagoHttpClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        });

        // ===== GEMINI =====
        var geminiSettings = configuration.GetSection("MachineLearning:Gemini");
        var geminiBaseUrl = geminiSettings["BaseUrl"];
        var apiVersion = geminiSettings["ApiVersion"];

        var geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        if (string.IsNullOrWhiteSpace(geminiApiKey))
            throw new Exception("GEMINI_API_KEY não configurada");

        services.AddHttpClient<IGeminiHttpClient, GeminiHttpClient>(client =>
        {
            // Base: https://generativelanguage.googleapis.com/v1beta/
            client.BaseAddress = new Uri($"{geminiBaseUrl}/{apiVersion}/");

            
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            
            client.DefaultRequestHeaders.Add("X-Goog-Api-Key", geminiApiKey);
        });
        // Places resolver
        services.AddHttpClient<IPlaceImageResolver, WikipediaImageResolver>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "EstudoIA-Tourism/1.0 (dev@local)"
            );
            client.Timeout = TimeSpan.FromSeconds(10);
        });




        services.AddScoped<TourismSummaryService>();
        return services;
    }
}
