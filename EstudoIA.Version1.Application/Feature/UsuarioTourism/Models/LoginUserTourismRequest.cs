using EstudoIA.Version1.Application.Abstractions.Handlers;

namespace EstudoIA.Version1.Application.Feature.UsuarioTourism.Models;

public sealed class LoginUserTourismRequest : IRequest<LoginUserTourismResponse>
{
    public string Email { get; init; } = default!;
    public string Senha { get; init; } = default!;
}
