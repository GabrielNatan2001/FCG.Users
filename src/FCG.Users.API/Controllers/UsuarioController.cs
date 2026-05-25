using FCG.Users.Application.Usuario.Dtos;
using FCG.Users.Application.Usuario.Services;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromServices] CriarUsuarioService service,
        [FromBody] CriarUsuarioDto.Request request)
    {
        var result = await service.Execute(request);
        return Created($"/api/usuario/{result.Id}", result);
    }
}
