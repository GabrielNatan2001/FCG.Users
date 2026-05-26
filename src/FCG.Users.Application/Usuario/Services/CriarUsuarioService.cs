using FCG.Users.Application.Messaging;
using FCG.Users.Application.Messaging.Events;
using FCG.Users.Application.Usuario.Dtos;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Usuario.Entities;
using FCG.Users.Domain.Usuario.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Users.Application.Usuario.Services;

public class CriarUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IMessageBus _messageBus;
    private readonly UserCreatedPublisherConfig _publisherConfig;
    private readonly ILogger<CriarUsuarioService> _logger;

    public CriarUsuarioService(
        IUsuarioRepository repository,
        IMessageBus messageBus,
        IOptions<UserCreatedPublisherConfig> publisherConfig,
        ILogger<CriarUsuarioService> logger)
    {
        _repository = repository;
        _messageBus = messageBus;
        _publisherConfig = publisherConfig.Value;
        _logger = logger;
    }

    public async Task<CriarUsuarioDto.Response> Execute(CriarUsuarioDto.Request request)
    {
        var existente = await _repository.ObterPorEmailAsync(request.Email);
        if (existente is not null)
            throw new DomainException("Email já cadastrado");

        var usuario = UsuarioEntity.Criar(request.Nome, request.Email, request.Senha);
        await _repository.Adicionar(usuario);
        await _repository.SalvarAlteracoes();

        _messageBus.Publish(
            _publisherConfig.Exchange,
            _publisherConfig.RoutingKey,
            new UserCreatedEvent(
                usuario.Id,
                usuario.Email.Value,
                usuario.Nome,
                DateTime.UtcNow));

        _logger.LogInformation("Usuário {UserId} criado e evento UserCreatedEvent publicado.", usuario.Id);

        return await Task.FromResult(new CriarUsuarioDto.Response(usuario.Id));
    }
}
