using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Services.Options
{
    public sealed class EvolutionWhatsAppOptions
    {
        public string ServerUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
    }
}