using EstudoIA.Version1.Application.Shared.HttpClients.Twilio;

namespace EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi
{
    public interface IWhatsAppEvolutionService
    {
        Task<WhatsAppSendResult> SendTextMessageAsync(
           string to,
           string message,
           string instanceName,
           CancellationToken cancellationToken = default);


        Task<WhatsAppInstanceCreateResult> CreateInstanceAsync(
            string instanceName,
            CancellationToken cancellationToken = default);

        Task<WhatsAppQrCodeResult> GetQrCodeAsync(
            string instanceName,
            CancellationToken cancellationToken = default);

        Task<WhatsAppConnectionStateResult> GetConnectionStateAsync(
            string instanceName,
            CancellationToken cancellationToken = default);

        Task<bool> DisconnectAsync(
            string instanceName,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteInstanceAsync(string instanceName, CancellationToken cancellationToken);
    }
}
