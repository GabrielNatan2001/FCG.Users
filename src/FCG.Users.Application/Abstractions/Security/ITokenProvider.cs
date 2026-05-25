using FCG.Users.Domain.Usuario.Entities;

namespace FCG.Users.Application.Abstractions.Security;

public interface ITokenProvider
{
    string GerarToken(UsuarioEntity usuario);
}
