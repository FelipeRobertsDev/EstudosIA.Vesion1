using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Services.Options
{
    public sealed class TwilioWhatsAppOptions
    {
        public const string SectionName = "TwilioWhatsApp";

        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
    }
}
