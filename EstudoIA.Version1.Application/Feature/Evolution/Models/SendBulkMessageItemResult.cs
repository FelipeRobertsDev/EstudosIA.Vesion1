using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Feature.Twilio.Models
{
    public class SendBulkMessageItemResult
    {
        public string? Nome { get; set; }

        public string? Celular { get; set; }

        public bool Success { get; set; }

        public string? MessageId { get; set; }

        public string? Error { get; set; }
    }
}
