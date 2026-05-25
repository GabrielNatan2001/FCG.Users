using FCG.Users.Application.Abstractions.Security;
using FCG.Users.Application.Usuario.Dtos;
using FCG.Users.Domain.Common.Enums;
using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Usuario.Interfaces;
using Microsoft.Extensions.Logging;

namespace FCG.Users.Application.Usuario.Services;

public class AutenticarUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<AutenticarUsuarioService> _logger;

    public AutenticarUsuarioService(
        IUsuarioRepository repository,
        ITokenProvider tokenProvider,
        ILogger<AutenticarUsuarioService> logger)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<AutenticarUsuarioDto.Response> Execute(AutenticarUsuarioDto.Request request)
    {
        var usuario = await _repository.ObterPorEmailAsync(request.Email);

        if (usuario is null || !usuario.Password.Verify(request.Senha))
            throw new DomainException("Email ou senha inválidos.");

        if (usuario.Status != EStatus.Ativo)
            throw new DomainException("Usuário inativo. Entre em contato com o suporte.");

        var accessToken = _tokenProvider.GerarToken(usuario);
        _logger.LogInformation("Usuário {Email} autenticado.", request.Email);

        return new AutenticarUsuarioDto.Response(accessToken, DateTime.UtcNow.AddHours(8));
    }
}
