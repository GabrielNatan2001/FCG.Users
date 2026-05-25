using FCG.Users.Domain.Usuario.Entities;

namespace FCG.Users.Domain.Usuario.Interfaces;

public interface IUsuarioRepository
{
    Task<UsuarioEntity?> ObterPorEmailAsync(string email);
    Task Adicionar(UsuarioEntity usuario);
    Task<int> SalvarAlteracoes();
}
