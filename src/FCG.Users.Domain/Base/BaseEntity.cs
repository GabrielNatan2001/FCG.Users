namespace FCG.Users.Domain.Base;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime DtCadastro { get; protected set; } = DateTime.UtcNow;
    public DateTime? DtAtualizacao { get; protected set; }

    public void AtualizarDataAtualizacao() => DtAtualizacao = DateTime.UtcNow;
}
