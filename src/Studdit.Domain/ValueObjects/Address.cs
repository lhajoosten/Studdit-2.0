using Ardalis.GuardClauses;
using Studdit.Domain.Common;
using Studdit.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Studdit.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }

        public Address(string street, string city, string postalCode)
        {
            if (!DutchPostalCodeRegex.IsMatch(postalCode))
            {
                throw new DomainException("Invalid Dutch postal code format. Expected format: '1234 AB'.");
            }

            Street = Guard.Against.NullOrWhiteSpace(street, nameof(street));
            City = Guard.Against.NullOrWhiteSpace(city, nameof(city));
            PostalCode = Guard.Against.NullOrWhiteSpace(postalCode, nameof(postalCode));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return PostalCode;
        }

        public override string ToString() => $"{Street}, {City}, {PostalCode}";

        private static readonly Regex DutchPostalCodeRegex = new(@"^\d{4}\s?[A-Z]{2}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
