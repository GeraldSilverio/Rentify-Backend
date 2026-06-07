namespace Rentify.Backend.Core.Domain.ValueObjects
{
    public sealed class PhoneNumber
    {
        public string Value { get; }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number is required.");

            value = value.Trim();

            // Validación simple
            if (value.Length < 7 || value.Length > 15)
                throw new ArgumentException("Invalid phone number length.");

            // Solo números y +
            if (!value.All(c => char.IsDigit(c) || c == '+'))
                throw new ArgumentException("Phone number contains invalid characters.");

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        protected bool Equals(PhoneNumber other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is PhoneNumber other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}