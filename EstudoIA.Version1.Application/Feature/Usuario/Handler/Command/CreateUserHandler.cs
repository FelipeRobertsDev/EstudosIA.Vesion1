using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.UserContext;
using EstudoIA.Version1.Application.Feature.Usuario.Models;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;

namespace EstudoIA.Version1.Application.Feature.Usuario.Handler.Command;

public class CreateUserHandler : HandlerBase<RegisterUserRequest, RegisterUserResponse>
{
    private readonly IUserDbContext _userDbContext;

    public CreateUserHandler(IUserDbContext userDbContext)
    {
        _userDbContext = userDbContext; 
    }

    protected override async Task<Result<RegisterUserResponse>> ExecuteAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = new Data.UserContext.Entities.UserContext
        {
            Name = request.Name,
            Email = request.Email
        };

        user.SetPassword(request.PasswordHash); 


        await _userDbContext.InsertAsync(user, cancellationToken);
        await _userDbContext.WriteChangesAsync(cancellationToken);

        var response = new RegisterUserResponse
        {
            Id = user.Id,
            Nome = user.Name
        };

        return Result<RegisterUserResponse>.Success(response);
    }


    protected override async Task<ValidationResult> ValidateAsync(
    RegisterUserRequest request,
    CancellationToken cancellationToken)
    {
        var validator = new RegisterUserRequestValidator();
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




    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório");

            RuleFor(x => x.PasswordHash)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");

            RuleFor(x => x.ConfirmPasswrd)
            .Equal(x => x.PasswordHash).WithMessage("As senhas não conferem");


            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório")
            .EmailAddress().WithMessage("E-mail inválido");


        }
    }

}


