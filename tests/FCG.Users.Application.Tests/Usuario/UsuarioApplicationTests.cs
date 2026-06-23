using FCG.Users.Application.Abstractions.Security;
using FCG.Users.Application.Messaging;
using FCG.Users.Application.Messaging.Events;
using FCG.Users.Application.Usuario.Dtos;
using FCG.Users.Application.Usuario.Services;
using FCG.Users.Domain.Common.Enums;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Usuario.Entities;
using FCG.Users.Domain.Usuario.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Users.Application.Tests.Usuario;

public class CriarUsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repository = new();
    private readonly Mock<IMessageBus> _messageBus = new();
    private readonly Mock<ILogger<CriarUsuarioService>> _logger = new();
    private readonly UserCreatedPublisherConfig _publisherConfig = new()
    {
        Exchange = "fcg.user.created",
        RoutingKey = "notifications.user-created"
    };

    private CriarUsuarioService CreateService() =>
        new(_repository.Object, _messageBus.Object, Options.Create(_publisherConfig), _logger.Object);

    [Fact]
    public async Task Execute_ComEmailJaCadastrado_DeveLancarDomainException()
    {
        _repository.Setup(r => r.ObterPorEmailAsync("existente@teste.com"))
            .ReturnsAsync(UsuarioEntity.Criar("Existente", "existente@teste.com", "Teste@123"));

        var service = CreateService();
        var request = new CriarUsuarioDto.Request("Novo", "existente@teste.com", "Teste@123");

        var ex = await Assert.ThrowsAsync<DomainException>(() => service.Execute(request));

        Assert.Equal("Email já cadastrado", ex.Message);
    }

    [Fact]
    public async Task Execute_ComDadosValidos_DevePersistirEPublicarEvento()
    {
        _repository.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UsuarioEntity?)null);
        _repository.Setup(r => r.SalvarAlteracoes()).ReturnsAsync(1);

        var service = CreateService();
        var request = new CriarUsuarioDto.Request("João", "joao@teste.com", "Teste@123");

        var response = await service.Execute(request);

        Assert.NotEqual(Guid.Empty, response.Id);
        _repository.Verify(r => r.Adicionar(It.IsAny<UsuarioEntity>()), Times.Once);
        _repository.Verify(r => r.SalvarAlteracoes(), Times.Once);
        _messageBus.Verify(m => m.Publish(
            _publisherConfig.Exchange,
            _publisherConfig.RoutingKey,
            It.IsAny<UserCreatedEvent>()), Times.Once);
    }
}

public class AutenticarUsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repository = new();
    private readonly Mock<ITokenProvider> _tokenProvider = new();
    private readonly Mock<ILogger<AutenticarUsuarioService>> _logger = new();

    private AutenticarUsuarioService CreateService() =>
        new(_repository.Object, _tokenProvider.Object, _logger.Object);

    [Fact]
    public async Task Execute_ComCredenciaisInvalidas_DeveLancarDomainException()
    {
        _repository.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UsuarioEntity?)null);

        var service = CreateService();
        var request = new AutenticarUsuarioDto.Request("inexistente@teste.com", "Teste@123");

        var ex = await Assert.ThrowsAsync<DomainException>(() => service.Execute(request));

        Assert.Equal("Email ou senha inválidos.", ex.Message);
    }

    [Fact]
    public async Task Execute_ComUsuarioInativo_DeveLancarDomainException()
    {
        var usuario = UsuarioEntity.Criar("Inativo", "inativo@teste.com", "Teste@123");
        SetStatus(usuario, EStatus.Inativo);

        _repository.Setup(r => r.ObterPorEmailAsync("inativo@teste.com"))
            .ReturnsAsync(usuario);

        var service = CreateService();
        var request = new AutenticarUsuarioDto.Request("inativo@teste.com", "Teste@123");

        var ex = await Assert.ThrowsAsync<DomainException>(() => service.Execute(request));

        Assert.Equal("Usuário inativo. Entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Execute_ComCredenciaisValidas_DeveRetornarToken()
    {
        var usuario = UsuarioEntity.Criar("João", "joao@teste.com", "Teste@123");

        _repository.Setup(r => r.ObterPorEmailAsync("joao@teste.com"))
            .ReturnsAsync(usuario);
        _tokenProvider.Setup(t => t.GerarToken(usuario)).Returns("token-jwt");

        var service = CreateService();
        var request = new AutenticarUsuarioDto.Request("joao@teste.com", "Teste@123");

        var response = await service.Execute(request);

        Assert.Equal("token-jwt", response.AccessToken);
        Assert.True(Math.Abs((response.ExpiraEmUtc - DateTime.UtcNow.AddHours(8)).TotalSeconds) <= 5);
    }

    private static void SetStatus(UsuarioEntity usuario, EStatus status)
    {
        typeof(UsuarioEntity).GetProperty(nameof(UsuarioEntity.Status))!
            .SetValue(usuario, status);
    }
}
