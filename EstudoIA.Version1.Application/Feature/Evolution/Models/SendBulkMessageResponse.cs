using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Feature.Twilio.Models
{
    public class SendBulkMessageResponse
    {
        public bool Success { get; set; }

        public int TotalRows { get; set; }

        public int SentCount { get; set; }

        public int FailedCount { get; set; }

        public List<SendBulkMessageItemResult> Results { get; set; } = new();
    }
}
