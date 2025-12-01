

using EstudoIA.Version1.Application.Abstractions.Handlers;

namespace EstudoIA.Version1.Application.Feature.Usuario.Models;

public class RegisterUserRequest : IRequest<RegisterUserResponse>
{
    public string Name { get; set; }
    public string PasswordHash { get; set; }

    public string Email { get; set; }

    public string ConfirmPasswrd { get; set; }

}

