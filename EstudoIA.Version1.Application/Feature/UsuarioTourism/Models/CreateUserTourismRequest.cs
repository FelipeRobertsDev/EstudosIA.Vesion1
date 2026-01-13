

using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Usuario.Models;

namespace EstudoIA.Version1.Application.Feature.UsuarioTourism.Models;

public sealed class CreateUserTourismRequest : IRequest<CreateUserTourismResponse>
{
    public string Nome { get; init; } = default!;
    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;
}
