using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Evolution.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EstudoIA.Version1.Application.Feature.Twilio.Handler.Command.CreateInstanceConnectionHandler;

namespace EstudoIA.Version1.Application.Feature.Evolution.Handler.Querie
{
    public class GetConnectionStateHandler : HandlerBase<GetConnectionStateRequest, GetConnectionStateResponse>
    {
        private readonly IWhatsAppEvolutionService _service;

        public GetConnectionStateHandler(IWhatsAppEvolutionService whatsAppEvolutionService)
        {
            _service = whatsAppEvolutionService;
        }

        protected override async Task<Result<GetConnectionStateResponse>> ExecuteAsync(
            GetConnectionStateRequest request,
            CancellationToken cancellationToken)
        {
            var stateResult = await _service.GetConnectionStateAsync(
                request.InstanceName,
                cancellationToken);

            var response = new GetConnectionStateResponse
            {
                Success = stateResult.Success,  
                InstanceName = request.InstanceName,
                State = stateResult.State,
                Connected = stateResult.Connected,
                Error = stateResult.Error
            };

            return Result<GetConnectionStateResponse>.Success(response);
        }
    

        protected async override Task<ValidationResult> ValidateAsync(GetConnectionStateRequest request, CancellationToken cancellationToken)
        {
            var validator = new GetConnectionRequestValidator();
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


        public class GetConnectionRequestValidator : AbstractValidator<GetConnectionStateRequest>
        {
            public GetConnectionRequestValidator()
            {
                RuleFor(x => x.InstanceName)
                    .NotNull()
                    .WithMessage("Instance name é obrigatória");
            }
        }
    }
}
