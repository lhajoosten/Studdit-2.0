using FluentAssertions;
using Studdit.Domain.Exceptions;
using Studdit.Domain.ValueObjects;

namespace Studdit.Domain.UnitTests.ValueObjects
{
    public class AddressTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateAddress()
        {
            // Arrange
            var street = "Kerkstraat 123";
            var city = "Amsterdam";
            var postalCode = "1234 AB";

            // Act
            var address = new Address(street, city, postalCode);

            // Assert
            address.Should().NotBeNull();
            address.Street.Should().Be(street);
            address.City.Should().Be(city);
            address.PostalCode.Should().Be(postalCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithInvalidStreet_ShouldThrowException(string street)
        {
            // Arrange
            var city = "Amsterdam";
            var postalCode = "1234 AB";

            // Act & Assert
            var action = () => new Address(street, city, postalCode);
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithInvalidCity_ShouldThrowException(string city)
        {
            // Arrange
            var street = "Kerkstraat 123";
            var postalCode = "1234 AB";

            // Act & Assert
            var action = () => new Address(street, city, postalCode);
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("123")]
        [InlineData("1234")]
        [InlineData("1234ABCD")]
        [InlineData("ABCD 1234")]
        public void Constructor_WithInvalidPostalCode_ShouldThrowDomainException(string postalCode)
        {
            // Arrange
            var street = "Kerkstraat 123";
            var city = "Amsterdam";

            // Act & Assert
            var action = () => new Address(street, city, postalCode);
            action.Should().Throw<DomainException>()
                .WithMessage("Invalid Dutch postal code format. Expected format: '1234 AB'.");
        }

        [Theory]
        [InlineData("1234AB")]   // Without space
        [InlineData("1234 ab")]  // Lowercase letters
        [InlineData("1234 AB")]  // Standard format
        [InlineData("9999ZZ")]   // No space, uppercase
        public void Constructor_WithValidDutchPostalCodeFormats_ShouldSucceed(string postalCode)
        {
            // Arrange
            var street = "Kerkstraat 123";
            var city = "Amsterdam";

            // Act
            var address = new Address(street, city, postalCode);

            // Assert
            address.PostalCode.Should().Be(postalCode);
        }

        [Theory]
        [InlineData("1234 ABC")] // Too many letters
        [InlineData("12345 AB")] // Too many digits
        [InlineData("123 AB")]   // Too few digits
        [InlineData("ABCD EF")]  // Letters instead of numbers
        [InlineData("1234")]     // Missing letters
        [InlineData("AB")]       // Only letters
        [InlineData("12345")]    // US format
        [InlineData("SW1A 1AA")] // UK format  
        [InlineData("10115")]    // German format
        [InlineData("75001")]    // French format
        [InlineData("ABC DEF")]  // Invalid format
        public void Constructor_WithInvalidDutchPostalCodeFormat_ShouldThrowDomainException(string postalCode)
        {
            // Arrange
            var street = "Kerkstraat 123";
            var city = "Amsterdam";

            // Act & Assert
            var action = () => new Address(street, city, postalCode);
            action.Should().Throw<DomainException>()
                .WithMessage("Invalid Dutch postal code format. Expected format: '1234 AB'.");
        }

        [Fact]
        public void Equals_WithSameAddressValues_ShouldReturnTrue()
        {
            // Arrange
            var address1 = new Address("Kerkstraat 123", "Amsterdam", "1234 AB");
            var address2 = new Address("Kerkstraat 123", "Amsterdam", "1234 AB");

            // Act & Assert
            address1.Should().Be(address2);
            (address1 == address2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentAddressValues_ShouldReturnFalse()
        {
            // Arrange
            var address1 = new Address("Kerkstraat 123", "Amsterdam", "1234 AB");
            var address2 = new Address("Hoofdstraat 456", "Utrecht", "5678 CD");

            // Act & Assert
            address1.Should().NotBe(address2);
            (address1 != address2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentStreet_ShouldReturnFalse()
        {
            // Arrange
            var address1 = new Address("Kerkstraat 123", "Amsterdam", "1234 AB");
            var address2 = new Address("Hoofdstraat 123", "Amsterdam", "1234 AB");

            // Act & Assert
            address1.Should().NotBe(address2);
        }

        [Fact]
        public void Equals_WithDifferentCity_ShouldReturnFalse()
        {
            // Arrange
            var address1 = new Address("Kerkstraat 123", "Amsterdam", "1234 AB");
            var address2 = new Address("Kerkstraat 123", "Utrecht", "1234 AB");

            // Act & Assert
            address1.Should().NotBe(address2);
        }

        [Fact]
        public void Equals_WithDifferentPostalCode_ShouldReturnFalse()
        {
            // Arrange
            var address1 = new Address("Kerkstraat 123", "Amsterdam", "1234 AB");
            var address2 = new Address("Kerkstraat 123", "Amsterdam", "5678 CD");

            // Act & Assert
            address1.Should().NotBe(address2);
        }

        [Fact]
        public void GetHashCode_WithSameAddress_ShouldReturnSameHashCode()
        {
            // Arrange
            var address1 = new Address("Kerkstraat 123", "Amsterdam", "1234 AB");
            var address2 = new Address("Kerkstraat 123", "Amsterdam", "1234 AB");

            // Act & Assert
            address1.GetHashCode().Should().Be(address2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnFormattedAddress()
        {
            // Arrange
            var street = "Kerkstraat 123";
            var city = "Amsterdam";
            var postalCode = "1234 AB";
            var address = new Address(street, city, postalCode);

            // Act
            var result = address.ToString();

            // Assert
            result.Should().Be($"{street}, {city}, {postalCode}");
        }

        [Theory]
        [InlineData("1234 AB")]
        [InlineData("5678 CD")]
        [InlineData("9999 ZZ")]
        [InlineData("0000 AA")]
        public void Constructor_WithValidDutchPostalCodes_ShouldSucceed(string postalCode)
        {
            // Arrange
            var street = "Teststraat 1";
            var city = "Teststad";

            // Act
            var address = new Address(street, city, postalCode);

            // Assert
            address.PostalCode.Should().Be(postalCode);
        }
    }
}