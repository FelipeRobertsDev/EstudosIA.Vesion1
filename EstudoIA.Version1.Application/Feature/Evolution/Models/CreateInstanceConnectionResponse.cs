using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Feature.Evolution.Models
{
    public class CreateInstanceConnectionResponse
    {
        public bool Success { get; set; }

        public string? InstanceName { get; set; }

        public string? QrCodeBase64 { get; set; }

        public string? QrCodeText { get; set; }

        public string? Error { get; set; }
    }
}
