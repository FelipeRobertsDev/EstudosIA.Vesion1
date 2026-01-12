//using EstudoIA.Version1.Application.Abstractions.Handlers;
//using EstudoIA.Version1.Application.Data.UserContext;
//using EstudoIA.Version1.Application.Feature.Usuario.Models;
//using EstudosIA.Version1.ApplicationCommon.Results;
//using FluentValidation;

//namespace EstudoIA.Version1.Application.Feature.Usuario.Handler.Querie;

//public class LoginUserHandler : HandlerBase<LoginUserRequest, LoginUserResponse>
//{
//    private readonly IUserDbContext _context;
//    private readonly JwtTokenService _jwtTokenService;

//    public LoginUserHandler(
//        IUserDbContext userDbContext,
//        JwtTokenService jwtTokenService)
//    {
//        _context = userDbContext;
//        _jwtTokenService = jwtTokenService;
//    }

//    protected override async Task<Result<LoginUserResponse>> ExecuteAsync(
//        LoginUserRequest request,
//        CancellationToken cancellationToken)
//    {
//        var user = await _context.GetByEmailAsync(request.Email, cancellationToken);

//        if (user == null || !user.VerifyPassword(request.Password))
//        {
//            return Result<LoginUserResponse>.Failure(
//                ErrorInfo.Create("Usuário ou senha inválidos", "INVALID_CREDENTIALS"));
//        }

//        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Name);

//        return Result<LoginUserResponse>.Success(new LoginUserResponse
//        {
//            AccesToken = token,
//            Name = user.Name
//        });
//    }

//    protected override async Task<ValidationResult> ValidateAsync(
//        LoginUserRequest request,
//        CancellationToken cancellationToken)
//    {
//        var validator = new LoginUserRequestValidator();
//        var result = await validator.ValidateAsync(request, cancellationToken);

//        if (!result.IsValid)
//        {
//            var errors = result.Errors
//                .Select(e => ErrorInfo.Create(e.ErrorMessage, e.PropertyName.ToUpperInvariant()))
//                .ToArray();

//            return ValidationResult.Failure(errors);
//        }

//        return ValidationResult.Success;
//    }

//    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
//    {
//        public LoginUserRequestValidator()
//        {
//            RuleFor(x => x.Email)
//                .NotEmpty().WithMessage("E-mail é obrigatório")
//                .EmailAddress().WithMessage("E-mail inválido");

//            RuleFor(x => x.Password)
//                .NotEmpty().WithMessage("Senha é obrigatória");
//        }
//    }
//}
