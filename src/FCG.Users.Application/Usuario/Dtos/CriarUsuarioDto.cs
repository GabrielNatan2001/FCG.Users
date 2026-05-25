namespace FCG.Users.Application.Usuario.Dtos;

public static class CriarUsuarioDto
{
    public record Request(string Nome, string Email, string Senha);
    public record Response(Guid Id);
}
