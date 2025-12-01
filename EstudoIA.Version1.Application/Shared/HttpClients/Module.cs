using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EstudoIA.Version1.Application.Abstractions.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var mercadoPagoSettings = configuration.GetSection("Payment:AbacatePay");
        var accessToken = mercadoPagoSettings["ChaveSecret"];
        var baseUrl = mercadoPagoSettings["BaseUrl"]; 

        services.AddHttpClient<IPaymentGatewayAbacatePayHttpClient, PaymentGatewayMercadoPagoHttpClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        });

        return services;
    }
}
