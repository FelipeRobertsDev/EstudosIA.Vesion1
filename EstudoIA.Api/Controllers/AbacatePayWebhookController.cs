using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Evolution.Models;
using EstudoIA.Version1.Application.Feature.Payment.Models.Webhook;
using EstudoIA.Version1.Application.Feature.Twilio.Models;
using EstudosIA.Version1.ApplicationCommon.Results.Extentions;
using Microsoft.AspNetCore.Mvc;
using static EstudoIA.Api.Controllers.AbacatePayWebhookController;

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


    [HttpPost("send-bulk")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendMessage(
    [FromForm] SendBulkMessageFormd form,
    CancellationToken cancellationToken = default)
    {
        var request = new SendBulkMessageRequest
        {
            ExcelFile = form.ExcelFile,
            MessageTemplate = form.MessageTemplate,
            InstanceName = form.InstanceName,
        };

        var result = await _handlers.SendAsync(request, cancellationToken);
        return Ok(result);
    }


    [HttpPost("create-instance")]
    public async Task<IActionResult> CreateInstance(
       [FromBody] CreateInstanceConnectionRequest request,
       CancellationToken cancellationToken = default
    )
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

    [HttpDelete("delete-instance")]
    public async Task<IActionResult> DeleteInstance(
    [FromBody] DeleteInstanceConnectionRequest request,
    CancellationToken cancellationToken = default)
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

    [HttpPost("connection-state")]
    public async Task<IActionResult> GetConnectionState(
    [FromBody] GetConnectionStateRequest request,
    CancellationToken cancellationToken = default)
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }
    public class SendBulkMessageFormd
    {
        public IFormFile ExcelFile { get; set; } = default!;
        public string MessageTemplate { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
    }

}
