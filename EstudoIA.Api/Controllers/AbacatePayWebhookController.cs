using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Payment.Models.Webhook;
using EstudosIA.Version1.ApplicationCommon.Results.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace EstudoIA.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class AbacatePayWebhookController(IHandlerCollection handlerCollection) : ControllerBase
{
    private readonly IHandlerCollection _handlers = handlerCollection;

    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook(
        [FromBody] AbacatePayWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

}
