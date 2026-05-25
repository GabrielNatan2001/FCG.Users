using FCG.Users.Domain.Base;
using FCG.Users.Domain.Common.Enums;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Usuario.ValueObjects;

namespace FCG.Users.Domain.Usuario.Entities;

public class UsuarioEntity : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public EmailVo Email { get; private set; } = null!;
    public SenhaVo Password { get; private set; } = null!;
    public EPerfil Role { get; private set; }
    public EStatus Status { get; private set; }

    protected UsuarioEntity() { }

    private UsuarioEntity(string nome, EmailVo email, SenhaVo password, EPerfil role, EStatus status)
    {
        Nome = nome;
        Email = email;
        Password = password;
        Role = role;
        Status = status;
    }

    public static UsuarioEntity Criar(string nome, string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome é obrigatório");

        return new UsuarioEntity(
            nome,
            new EmailVo(email),
            SenhaVo.Create(senha),
            EPerfil.User,
            EStatus.Ativo);
    }

    public static UsuarioEntity CriarAdmin(string nome, string email, string senha)
    {
        var usuario = Criar(nome, email, senha);
        usuario.Role = EPerfil.Admin;
        return usuario;
    }
}
