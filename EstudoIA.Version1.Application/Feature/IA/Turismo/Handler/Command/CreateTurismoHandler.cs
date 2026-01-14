using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.IA;
using EstudoIA.Version1.Application.Shared.HttpClients.IA.Gemini;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;

namespace EstudoIA.Version1.Application.Feature.IA.Turismo.Handler.Command;

public class CreateTurismoHandler : HandlerBase<TourismSummaryRequest, TourismSummaryResponse>
{
   
    private readonly TourismSummaryService _service;

    public CreateTurismoHandler(TourismSummaryService tourismSummaryService)
    {
        _service = tourismSummaryService;
    }

    protected override async Task<Result<TourismSummaryResponse>> ExecuteAsync(TourismSummaryRequest request, CancellationToken cancellationToken)
    {

        var response = await _service.GetSummaryWithImagesAsync(request, cancellationToken);
        return Result<TourismSummaryResponse>.Success(response);
    }

    protected override async Task<ValidationResult> ValidateAsync(TourismSummaryRequest request, CancellationToken cancellationToken)
    {
        var validator = new TourismRequestValidator();
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


    public class TourismRequestValidator : AbstractValidator<TourismSummaryRequest>
    {
        public TourismRequestValidator()
        {
            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("O país é obrigatório.")
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("A cidade é obrigatória.")
                .MaximumLength(100);


            RuleFor(x => x.Language)
                .NotEmpty()
                .WithMessage("O idioma é obrigatório.")
                .Matches(@"^[a-z]{2}(-[A-Z]{2})?$")
                .WithMessage("Formato de idioma inválido. Ex: pt-BR, en-US.");

            // Opcionais, mas com regras se vierem preenchidos
            RuleFor(x => x.TravelerProfile)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.TravelerProfile));

            RuleFor(x => x.Budget)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.Budget));
        }
    }

}
