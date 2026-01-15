using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.UserTripPlans;
using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.IA;
using EstudoIA.Version1.Application.Shared.HttpClients.IA.Gemini;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EstudoIA.Version1.Application.Feature.IA.Turismo.Handler.Command;

public class CreateTurismoHandler : HandlerBase<TourismSummaryRequest, TourismSummaryResponse>
{
   
    private readonly TourismSummaryService _service;
    private readonly IUserTripPlansContext _userTripPlansContext;


    public CreateTurismoHandler(TourismSummaryService tourismSummaryService, IUserTripPlansContext userTripPlansContext)
    {
        _service = tourismSummaryService;
        _userTripPlansContext = userTripPlansContext;
    }

    protected override async Task<Result<TourismSummaryResponse>> ExecuteAsync(
    TourismSummaryRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _service.GetSummaryWithImagesAsync(request, cancellationToken);

        // ✅ Garante que cada spot tenha um GUID
        if (response?.Spots is not null)
        {
            foreach (var spot in response.Spots)
            {
                if (spot.Id == Guid.Empty)
                    spot.Id = Guid.NewGuid();

                // opcional: garantir isInRoute default
                // spot.IsInRoute ??= false; // se for bool? (nullable)
            }
        }

        // 1) Serializa o response que você vai retornar
        var routeDoc = JsonSerializer.SerializeToDocument(response);

        // 2) Garante Id / IsInRoute no JSON (camel e pascal)
        var fixedDoc = EnsureSpotIdAndInRoute(routeDoc);

        // 3) Salva o JSON corrigido
        await _userTripPlansContext.UpsertRouteAsync(
            userId: request.UserId,
            city: response.City ?? request.City,
            country: response.Country ?? request.Country,
            route: fixedDoc,
            cancellationToken: cancellationToken);

        return Result<TourismSummaryResponse>.Success(response);
    }


    private static JsonDocument EnsureSpotIdAndInRoute(JsonDocument doc)
    {
        var root = JsonNode.Parse(doc.RootElement.GetRawText()) as JsonObject;
        if (root is null) return doc;

        // spots pode ser "spots" ou "Spots"
        var spotsNode = root["spots"] ?? root["Spots"];
        if (spotsNode is not JsonArray spotsArray) return doc;

        foreach (var item in spotsArray)
        {
            if (item is not JsonObject spotObj) continue;

            // Id / id
            var idNode = spotObj["id"] ?? spotObj["Id"];
            if (idNode is null || !Guid.TryParse(idNode.ToString(), out _))
            {
                // mantém o padrão do JSON que já veio (camelCase)
                if (spotObj.ContainsKey("id")) spotObj["id"] = Guid.NewGuid().ToString();
                else spotObj["Id"] = Guid.NewGuid().ToString();
            }

            // IsInRoute / isInRoute
            var inRouteNode = spotObj["isInRoute"] ?? spotObj["IsInRoute"];
            if (inRouteNode is null)
            {
                if (spotObj.ContainsKey("isInRoute")) spotObj["isInRoute"] = false;
                else spotObj["IsInRoute"] = false;
            }
        }

        return JsonDocument.Parse(root.ToJsonString());
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
