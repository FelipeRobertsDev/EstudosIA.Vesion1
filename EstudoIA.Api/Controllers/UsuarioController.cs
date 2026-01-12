using EstudoIA.Version1.Application.Abstractions.Handlers;

using EstudoIA.Version1.Application.Feature.Usuario.Models;
using EstudosIA.Version1.ApplicationCommon.Results.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace EstudoIA.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class UsuarioController(IHandlerCollection handlerCollection) : ControllerBase
{
    private readonly IHandlerCollection _handlers = handlerCollection;

    //[HttpPost("register")]
    //public async Task<IActionResult> PostRegisterUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken = default)
    //{
    //    var result = await _handlers.SendAsync(request, cancellationToken);
    //    return result.AsActionResult();
    //}


    //[HttpPost("login")]
    //public async Task<IActionResult> PostLoginUser([FromBody] LoginUserRequest request, CancellationToken cancellationToken = default)
    //{
    //    var result = await _handlers.SendAsync(request, cancellationToken);
    //    return result.AsActionResult();
    //}
}
