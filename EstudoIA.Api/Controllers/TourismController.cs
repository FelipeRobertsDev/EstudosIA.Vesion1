using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;
using EstudoIA.Version1.Application.Feature.UsuarioTourism.Models;
using EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;
using EstudosIA.Version1.ApplicationCommon.Results.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace EstudoIA.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TourismController(IHandlerCollection handlerCollection) : ControllerBase
{
    private readonly IHandlerCollection _handlers = handlerCollection;

    [HttpPost("tourism")]
    public async Task<IActionResult> PostTourism(
        [FromBody] TourismSummaryRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

    [HttpPost("create/user")]
    public async Task<IActionResult> PostCreateUser(
        [FromBody] CreateUserTourismRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

    [HttpGet("login")]
    public async Task<IActionResult> GetLogin(
        [FromQuery] string email,
        [FromQuery] string password,
        CancellationToken cancellationToken = default)
    {
        var request = new LoginUserTourismRequest
        {
            Email = email,
            Senha = password
        };

        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

    [HttpGet("user-trip")]
    public async Task<IActionResult> GetUserTrip(
        [FromQuery] Guid id,
        CancellationToken cancellationToken = default)
    {
        var request = new GetUserTripPlanRequest
        {
            UserId = id
        };

        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }


    [HttpGet("user-inroute")]
    public async Task<IActionResult> GetUserTripInRoute(
       [FromQuery] Guid id,
       CancellationToken cancellationToken = default)
    {
        var request = new GetUserTripPlanInRouteRequest
        {
            Id = id
        };

        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }

    // 🔥 NOVO ENDPOINT: atualiza isInRoute de um spot
    [HttpPatch("user-trip/spot")]
    public async Task<IActionResult> PatchUserTripSpot(
        [FromBody] UpdateSpotInRouteRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _handlers.SendAsync(request, cancellationToken);
        return result.AsActionResult();
    }
}
