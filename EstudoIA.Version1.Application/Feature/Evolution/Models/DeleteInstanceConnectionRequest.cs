

using EstudoIA.Version1.Application.Abstractions.Handlers;

namespace EstudoIA.Version1.Application.Feature.Evolution.Models;

public class DeleteInstanceConnectionRequest : IRequest<DeleteInstanceConnectionResponse>
{
    public string InstanceName { get; set; } = string.Empty;
}
