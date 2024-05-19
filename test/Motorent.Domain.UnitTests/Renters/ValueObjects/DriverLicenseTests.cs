using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.UnitTests.Renters.ValueObjects;

[TestSubject(typeof(DriverLicense))]
public sealed class DriverLicenseTests
{
    private const string Number = "83907030608";
    private static readonly DateOnly Expiry = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
    private static readonly DriverLicenseCategory Category = DriverLicenseCategory.AB;
    
    public static readonly IEnumerable<object[]> ValidDriverLicenseNumbers = new List<object[]>
    {
        new object[] { "83907030608" },
        new object[] { "50542712396" },
        new object[] { "84407903668" },
        new object[] { "26775490609" },
        new object[] { "00351664298" }
    };

    public static readonly IEnumerable<object[]> InvalidDriverLicenseNumbers = new List<object[]>
    {
        new object[] { "2677549060" },
        new object[] { "00351864298" },
        new object[] { "11111111111" }
    };
    
    [Fact]
    public void Create_WhenValuesAreValid_ShouldReturnDriverLicense()
    {
        // Arrange
        // Act
        var result = DriverLicense.Create(Number, Category, Expiry);

        // Assert
        result.Should().BeSuccess();
        
        result.Value.Number.Should().Be(Number);
        result.Value.Expiry.Should().Be(Expiry);
        result.Value.Category.Should().Be(Category);
    }

    [Theory, MemberData(nameof(ValidDriverLicenseNumbers))]
    public void Create_WhenNumberIsValid_ShouldReturnDriverLicense(string number)
    {
        // Arrange
        // Act
        var result = DriverLicense.Create(number, Category, Expiry);

        // Assert
        result.Should().BeSuccess(number);
    }

    [Theory, MemberData(nameof(InvalidDriverLicenseNumbers))]
    public void Create_WhenNumberIsInvalid_ShouldReturnInvalid(string number)
    {
        // Arrange
        // Act
        var result = DriverLicense.Create(number, Category, Expiry);

        // Assert
        result.Should().BeFailure(DriverLicense.Invalid);
    }

    [Fact]
    public void Create_WhenIsExpired_ShouldReturnExpired()
    {
        // Arrange
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var result = DriverLicense.Create(Number, Category, expirationDate);

        // Assert
        result.Should().BeFailure(DriverLicense.Expired);
    }
}