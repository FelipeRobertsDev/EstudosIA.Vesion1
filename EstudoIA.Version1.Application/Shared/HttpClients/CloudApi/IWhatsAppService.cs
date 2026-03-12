using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Shared.HttpClients.CloudApi
{
    public interface IWhatsAppService
    {
        Task<WhatsAppSendResult> SendTextMessageAsync(
            string to,
            string message,
            CancellationToken cancellationToken = default);

        Task<WhatsAppSendResult> SendTemplateMessageAsync(
            string to,
            string templateName,
            string languageCode,
            IEnumerable<string>? bodyParameters = null,
            CancellationToken cancellationToken = default);
    }
}
