

namespace EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;
public enum PaymentMethod 
{
    Pix,
    CreditCard,
    Boleto
}
public enum PaymentStatus
{
    Pending,
    Paid,
    Cancelled,
    Expired,
    Failed
}


public class PaymentGatewayMercadoPagoRequest
{
    // Identificador do pedido no seu sistema
    public string ExternalId { get; set; } = null!;

    // Produtos / plano escolhido
    public List<PaymentProductRequest> Products { get; set; } = new();

    // Métodos permitidos
    public List<PaymentMethod> Methods { get; set; } = new();

    // Dados do cliente
    public PaymentCustomerRequest Customer { get; set; } = null!;

    // URLs
    public string ReturnUrl { get; set; } = null!;
    public string CompletionUrl { get; set; } = null!;

    // Extra / opcional
    public Dictionary<string, string>? Metadata { get; set; }
}

public class PaymentProductRequest
{
    public string ExternalId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    // Quantidade normalmente 1 para planos
    public int Quantity { get; set; } = 1;

    // Valor em centavos (2990 = R$ 29,90)
    public int PriceInCents { get; set; }
}

public class PaymentCustomerRequest
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Cellphone { get; set; } = null!;
    public string TaxId { get; set; } = null!; // CPF ou CNPJ
}


public class PaymentResponse
{
    // ID da cobrança no gateway
    public string PaymentId { get; set; } = null!;

    // ID da cobrança no seu sistema
    public string ExternalId { get; set; } = null!;

    // Status atual
    public PaymentStatus Status { get; set; }

    // Valor total em centavos
    public int AmountInCents { get; set; }

    // Link de checkout
    public string CheckoutUrl { get; set; } = null!;

    // Métodos disponíveis
    public List<PaymentMethod> Methods { get; set; } = new();

    // Data de criação
    public DateTime CreatedAt { get; set; }
}


