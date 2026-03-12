using EstudoIA.Version1.Application.Abstractions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Feature.Evolution.Models
{
    public class GetConnectionStateRequest : IRequest<GetConnectionStateResponse>
    {
        public string InstanceName { get; set; } = string.Empty;
    }
}
