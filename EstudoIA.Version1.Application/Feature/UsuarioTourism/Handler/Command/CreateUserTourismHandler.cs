using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.UserTourism;
using EstudoIA.Version1.Application.Feature.UsuarioTourism.Models;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;
using Google.Cloud.Firestore;
using System.Text;

namespace EstudoIA.Version1.Application.Feature.UsuarioTourism.Handler.Command;

public class CreateUserTourismHandler : HandlerBase<CreateUserTourismRequest, CreateUserTourismResponse>
{
    private readonly IUserTourismContext _userTourismContext;

    public CreateUserTourismHandler(IUserTourismContext userTourismContext)
    {
        _userTourismContext = userTourismContext;
    }

    protected override async Task<Result<CreateUserTourismResponse>> ExecuteAsync(
        CreateUserTourismRequest request,
        CancellationToken cancellationToken)
    {
        var user = new Data.UserTourism.Entities.UserContextTourism
        {
            Name = request.Nome,
            Email = request.Email
        };


        user.SetPassword(request.Password);

        // Cria se não existir (ou retorna o existente)
        await _userTourismContext.InsertAsync(user, cancellationToken);
        await _userTourismContext.WriteChangesAsync(cancellationToken);

        var response = new CreateUserTourismResponse
        {
            
            Nome = request.Nome,
            Email = request.Email,

        };

        return Result<CreateUserTourismResponse>.Success(response);
    }

    protected override async Task<ValidationResult> ValidateAsync(
        CreateUserTourismRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new CreateUserTourismRequestValidator();
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var errors = result.Errors
                .Select(e => ErrorInfo.Create(e.ErrorMessage, e.PropertyName.ToUpperInvariant()))
                .ToArray();

            return ValidationResult.Failure(errors);
        }

        return ValidationResult.Success;
    }

    private static string EmailToDocId(string email)
    {
        var bytes = Encoding.UTF8.GetBytes(email);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public class CreateUserTourismRequestValidator : AbstractValidator<CreateUserTourismRequest>
    {
        public CreateUserTourismRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório")
                .EmailAddress().WithMessage("E-mail inválido");
        }
    }
}
