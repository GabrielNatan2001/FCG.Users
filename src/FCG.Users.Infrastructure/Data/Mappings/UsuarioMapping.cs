using FCG.Users.Domain.Usuario.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Users.Infrastructure.Data.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<UsuarioEntity>
{
    public void Configure(EntityTypeBuilder<UsuarioEntity> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("id").HasColumnType("uuid");
        builder.Property(u => u.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(u => u.Status).HasColumnName("status").IsRequired();

        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value).HasColumnName("email").HasMaxLength(150).IsRequired();
            email.HasIndex(e => e.Value).IsUnique();
        });

        builder.OwnsOne(u => u.Password, password =>
        {
            password.Property(p => p.Hash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        });

        builder.Property(e => e.DtCadastro).HasColumnName("dt_cadastro").IsRequired();
        builder.Property(e => e.DtAtualizacao).HasColumnName("dt_atualizacao");
    }
}
