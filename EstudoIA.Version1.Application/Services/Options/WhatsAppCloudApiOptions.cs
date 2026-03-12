using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Services.Options
{
    public sealed class WhatsAppCloudApiOptions
    {
        public const string SectionName = "WhatsAppCloudApi";

        public string BaseUrl { get; set; } = "https://graph.facebook.com";
        public string ApiVersion { get; set; } = "v23.0";
        public string PhoneNumberId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
