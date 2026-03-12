using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Usuario.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Feature.Twilio.Models
{
    public class SendBulkMessageRequest : IRequest<SendBulkMessageResponse>
    {
        public IFormFile ExcelFile { get; set; } = default!;

        public string MessageTemplate { get; set; } = string.Empty;

        public string InstanceName { get; set; } = string.Empty;
    }
}
