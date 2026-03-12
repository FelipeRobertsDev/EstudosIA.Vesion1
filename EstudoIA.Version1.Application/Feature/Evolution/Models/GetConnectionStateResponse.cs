using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Feature.Evolution.Models
{
    public class GetConnectionStateResponse
    {
        public bool Success { get; set; }

        public string? InstanceName { get; set; }

        public string? State { get; set; }

        public bool Connected { get; set; }

        public string? Error { get; set; }
    }
}
