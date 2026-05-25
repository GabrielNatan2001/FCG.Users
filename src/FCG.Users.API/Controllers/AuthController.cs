using FCG.Users.Application.Usuario.Dtos;
using FCG.Users.Application.Usuario.Services;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromServices] AutenticarUsuarioService service,
        [FromBody] AutenticarUsuarioDto.Request request)
    {
        var result = await service.Execute(request);
        return Ok(result);
    }
}
