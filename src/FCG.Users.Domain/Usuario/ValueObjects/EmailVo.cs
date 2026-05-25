using System.Text.RegularExpressions;
using FCG.Users.Domain.Exceptions;

namespace FCG.Users.Domain.Usuario.ValueObjects;

public class EmailVo
{
    public string Value { get; private set; } = string.Empty;

    protected EmailVo() { }

    public EmailVo(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email não pode ser vazio");

        value = value.Trim().ToLower();

        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("Email inválido");

        Value = value;
    }
}
