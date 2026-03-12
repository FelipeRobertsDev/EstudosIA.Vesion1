using EstudoIA.Version1.Application.Shared.HttpClients.CloudApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Shared.HttpClients.Twilio
{
    public interface IWhatsAppTwilioService
    {
        Task<WhatsAppSendResult> SendTextMessageAsync(
            string to,
            string message,
            CancellationToken cancellationToken = default);
    }
}
