namespace Rentify.Backend.Core.Domain.ValueObjects
{
    public sealed class Address
    {
        public string Street { get; }
        public string City { get; }
        public string Country { get; }

        public Address(
            string street,
            string city,
            string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street is required.");

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.");

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.");

            Street = street.Trim();
            City = city.Trim();
            Country = country.Trim();
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {Country}";
        }

        protected bool Equals(Address other)
        {
            return Street == other.Street &&
                   City == other.City &&
                   Country == other.Country;
        }

        public override bool Equals(object? obj)
        {
            return obj is Address other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street, City, Country);
        }
    }
}