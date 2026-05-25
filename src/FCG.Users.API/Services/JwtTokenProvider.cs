using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FCG.Users.Application.Abstractions.Security;
using FCG.Users.Domain.Usuario.Entities;
using Microsoft.IdentityModel.Tokens;

namespace FCG.Users.API.Services;

public class JwtTokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;

    public JwtTokenProvider(IConfiguration configuration) => _configuration = configuration;

    public string GerarToken(UsuarioEntity usuario)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "FCG.Users.API";
        var audience = _configuration["Jwt:Audience"] ?? "FCG.Client";
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email.Value),
            new(ClaimTypes.Role, usuario.Role.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
