using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Payment.Models;
using EstudosIA.Version1.ApplicationCommon.Results.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace EstudoIA.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class PaymentAbacatePayController(IHandlerCollection handlerCollection) : ControllerBase
{
    private readonly IHandlerCollection _handlers = handlerCollection;

    [HttpPost("charge")]
    public async Task<IActionResult> PostChargePayment([FromBody] CreatePaymentAbacatePayCommand request, CancellationToken cancellationToken = default)
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

}
