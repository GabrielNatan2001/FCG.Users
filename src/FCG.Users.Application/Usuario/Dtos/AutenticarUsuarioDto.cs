namespace FCG.Users.Application.Usuario.Dtos;

public static class AutenticarUsuarioDto
{
    public record Request(string Email, string Senha);
    public record Response(string AccessToken, DateTime ExpiraEmUtc);
}
