using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;
using EstudoIA.Version1.Application.Feature.Payment.Models;
using EstudosIA.Version1.ApplicationCommon.Results.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace EstudoIA.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class TourismController(IHandlerCollection handlerCollection) : ControllerBase
{
    private readonly IHandlerCollection _handlers = handlerCollection;

    [HttpPost("tourism")]
    public async Task<IActionResult> PostTourism([FromBody] TourismSummaryRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

}
