using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;

namespace EstudoIA.Version1.Application.Data.PaymentContext.Entities;

public class PaymentContext
{
    // =============================
    // Identidade
    // =============================

    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identificador do pagamento no seu sistema
    /// Ex: order_app_test_xxx
    /// </summary>
    public string ExternalId { get; set; } = null!;

    /// <summary>
    /// Identificador retornado pelo gateway
    /// Ex: bill_xxx
    /// </summary>
    public string GatewayPaymentId { get; set; } = null!;

    /// <summary>
    /// Nome do gateway (AbacatePay, MercadoPago, Stripe...)
    /// </summary>
    public string Gateway { get; set; } = "AbacatePay";

    // =============================
    // Pagamento
    // =============================

    public PaymentStatus Status { get; set; }

    public int AmountInCents { get; set; }

    /// <summary>
    /// URL do checkout retornada pelo gateway
    /// </summary>
    public string CheckoutUrl { get; set; } = null!;

    /// <summary>
    /// Métodos permitidos (PIX / CARD)
    /// Armazenado como string separada por vírgula
    /// </summary>
    public string Methods { get; set; } = null!;

    // =============================
    // Cliente
    // =============================

    public string CustomerName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public string CustomerTaxId { get; set; } = null!;
    public string CustomerCellphone { get; set; } = null!;

    // =============================
    // Datas
    // =============================

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // =============================
    // Helpers de domínio
    // =============================

    public void MarkAsPaid()
    {
        Status = PaymentStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCancelled()
    {
        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
