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

        // ===== GEMINI (OpenRouter por baixo) =====
        var geminiSettings = configuration.GetSection("MachineLearning:Gemini");

        var geminiBaseUrl = geminiSettings["BaseUrl"];
        if (string.IsNullOrWhiteSpace(geminiBaseUrl))
            throw new Exception("MachineLearning:Gemini:BaseUrl não configurada");

        // Mantendo o nome que você já usa
        var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("GEMINI_API_KEY não configurada");

        // Opcional, mas recomendado
        var referer =
            Environment.GetEnvironmentVariable("OPENROUTER_REFERER")
            ?? geminiSettings["Referer"]
            ?? "http://localhost";

        var appTitle =
            Environment.GetEnvironmentVariable("OPENROUTER_APP_TITLE")
            ?? geminiSettings["AppTitle"]
            ?? "EstudoIA";

        services.AddHttpClient<IGeminiHttpClient, GeminiHttpClient>(client =>
        {
            client.BaseAddress = new Uri(geminiBaseUrl);

            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.TryAddWithoutValidation("HTTP-Referer", referer);
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Title", appTitle);
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
