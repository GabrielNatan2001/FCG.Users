using FCG.Users.Domain.Exceptions;

namespace FCG.Users.Domain.Usuario.ValueObjects;

public class SenhaVo
{
    public string Hash { get; private set; } = string.Empty;

    protected SenhaVo() { }

    private SenhaVo(string hash) => Hash = hash;

    public static SenhaVo Create(string plainPassword)
    {
        if (!IsValid(plainPassword))
            throw new DomainException("Senha deve ter no mínimo 8 caracteres com letras, números e especiais");

        return new SenhaVo(BCrypt.Net.BCrypt.HashPassword(plainPassword));
    }

    public bool Verify(string plainPassword) => BCrypt.Net.BCrypt.Verify(plainPassword, Hash);

    private static bool IsValid(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        return password.Any(char.IsLetter)
            && password.Any(char.IsDigit)
            && password.Any(ch => !char.IsLetterOrDigit(ch));
    }
}
