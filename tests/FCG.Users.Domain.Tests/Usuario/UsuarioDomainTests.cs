using FCG.Users.Domain.Common.Enums;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Usuario.Entities;
using FCG.Users.Domain.Usuario.ValueObjects;

namespace FCG.Users.Domain.Tests.Usuario;

public class EmailVoTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_ComEmailVazio_DeveLancarDomainException(string email)
    {
        var ex = Assert.Throws<DomainException>(() => new EmailVo(email));

        Assert.Equal("Email não pode ser vazio", ex.Message);
    }

    [Theory]
    [InlineData("invalido")]
    [InlineData("invalido@")]
    [InlineData("@dominio.com")]
    public void Construtor_ComEmailInvalido_DeveLancarDomainException(string email)
    {
        var ex = Assert.Throws<DomainException>(() => new EmailVo(email));

        Assert.Equal("Email inválido", ex.Message);
    }

    [Fact]
    public void Construtor_ComEmailValido_DeveNormalizarParaMinusculas()
    {
        var email = new EmailVo("  Usuario@Exemplo.COM  ");

        Assert.Equal("usuario@exemplo.com", email.Value);
    }
}

public class SenhaVoTests
{
    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("abcdefgh")]
    [InlineData("12345678")]
    [InlineData("Abcdefgh")]
    public void Create_ComSenhaInvalida_DeveLancarDomainException(string senha)
    {
        var ex = Assert.Throws<DomainException>(() => SenhaVo.Create(senha));

        Assert.Equal("Senha deve ter no mínimo 8 caracteres com letras, números e especiais", ex.Message);
    }

    [Fact]
    public void Create_ComSenhaValida_DeveGerarHash()
    {
        var senha = SenhaVo.Create("Teste@123");

        Assert.False(string.IsNullOrEmpty(senha.Hash));
    }

    [Fact]
    public void Verify_ComSenhaCorreta_DeveRetornarTrue()
    {
        var senha = SenhaVo.Create("Teste@123");

        Assert.True(senha.Verify("Teste@123"));
    }

    [Fact]
    public void Verify_ComSenhaIncorreta_DeveRetornarFalse()
    {
        var senha = SenhaVo.Create("Teste@123");

        Assert.False(senha.Verify("Outra@123"));
    }
}

public class UsuarioEntityTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeVazio_DeveLancarDomainException(string nome)
    {
        var ex = Assert.Throws<DomainException>(() => UsuarioEntity.Criar(nome, "usuario@teste.com", "Teste@123"));

        Assert.Equal("Nome é obrigatório", ex.Message);
    }

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarUsuarioComPerfilUser()
    {
        var usuario = UsuarioEntity.Criar("João Silva", "joao@teste.com", "Teste@123");

        Assert.Equal("João Silva", usuario.Nome);
        Assert.Equal("joao@teste.com", usuario.Email.Value);
        Assert.Equal(EPerfil.User, usuario.Role);
        Assert.Equal(EStatus.Ativo, usuario.Status);
        Assert.NotEqual(Guid.Empty, usuario.Id);
    }

    [Fact]
    public void CriarAdmin_ComDadosValidos_DeveCriarUsuarioComPerfilAdmin()
    {
        var usuario = UsuarioEntity.CriarAdmin("Admin", "admin@teste.com", "Teste@123");

        Assert.Equal(EPerfil.Admin, usuario.Role);
    }
}
