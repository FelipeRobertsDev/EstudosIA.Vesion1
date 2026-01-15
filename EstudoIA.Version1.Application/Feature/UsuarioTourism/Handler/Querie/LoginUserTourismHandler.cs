using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.UserTourism;
using EstudoIA.Version1.Application.Feature.UsuarioTourism.Models;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;

namespace EstudoIA.Version1.Application.Feature.UsuarioTourism.Handler.Querie
{
    public class LoginUserTourismHandler : HandlerBase<LoginUserTourismRequest, LoginUserTourismResponse>
    {
        private readonly IUserTourismContext _userTourismContext;

        public LoginUserTourismHandler(IUserTourismContext userTourismContext)
        {
            _userTourismContext = userTourismContext;
        }

        protected override async Task<Result<LoginUserTourismResponse>> ExecuteAsync(LoginUserTourismRequest request, CancellationToken cancellationToken)
        {
            var user = await _userTourismContext.GetByEmailAsync(request.Email, cancellationToken);
            if(user == null || !user.VerifyPassword(request.Senha))
            {
                return Result<LoginUserTourismResponse>.Failure(ErrorInfo.Create("Usuário ou senha inválidos", "INVALID_CREDENTIALS"));
            }

            return Result<LoginUserTourismResponse>.Success(new LoginUserTourismResponse
            {
                Name = user.Name,
                Id = user.Id

            });
        }

        protected override async Task<ValidationResult> ValidateAsync(
            LoginUserTourismRequest request,
            CancellationToken cancellationToken)
        {
            var validator = new LoginUserTourismRequestValidator();
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

        public class LoginUserTourismRequestValidator : AbstractValidator<LoginUserTourismRequest>
        {
            public LoginUserTourismRequestValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("E-mail é obrigatório")
                    .EmailAddress().WithMessage("E-mail inválido");

                RuleFor(x => x.Senha)
                    .NotEmpty().WithMessage("Senha é obrigatória")
                    .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");
            }
        }
    }
}
