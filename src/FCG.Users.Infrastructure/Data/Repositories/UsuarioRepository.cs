using FCG.Users.Domain.Usuario.Entities;
using FCG.Users.Domain.Usuario.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.Data.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context) => _context = context;

    public async Task<UsuarioEntity?> ObterPorEmailAsync(string email) =>
        await _context.Usuarios.FirstOrDefaultAsync(x => x.Email.Value == email.ToLower());

    public async Task Adicionar(UsuarioEntity usuario) =>
        await _context.Usuarios.AddAsync(usuario);

    public async Task<int> SalvarAlteracoes() =>
        await _context.SaveChangesAsync();
}
