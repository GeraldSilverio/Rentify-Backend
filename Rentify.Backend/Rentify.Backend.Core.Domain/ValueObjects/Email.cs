namespace Rentify.Backend.Core.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email is required.");

        value = value.Trim();

        if (value.Length > 255)
            throw new ArgumentException("Email is too long.");

        if (!value.Contains("@"))
            throw new ArgumentException("Invalid email.");

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }

    protected bool Equals(Email other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Email other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }
}