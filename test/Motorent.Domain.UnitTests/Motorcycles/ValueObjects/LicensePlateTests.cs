using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.UnitTests.Motorcycles.ValueObjects;

[TestSubject(typeof(LicensePlate))]
public sealed class LicensePlateTests
{
    public static IEnumerable<object[]> ValidLicensePlates => new List<object[]>
    {
        new object[] { "KFE-7A64" },
        new object[] { "mnc-6B86" },
        new object[] { "LXP6A70" },
        new object[] { "kil6A04" }
    };
    
    public static IEnumerable<object[]> InvalidLicensePlates => new List<object[]>
    {
        new object[] { string.Empty },
        new object[] { "          " },
        new object[] { "KF-7A64" },
        new object[] { "mn16B86" },
        new object[] { "LXP6A7" },
        new object[] { "kil6204" }
    };
    
    [Theory, MemberData(nameof(ValidLicensePlates))]
    public void Create_WhenValueIsValid_ShouldReturnSuccess(string value)
    {
        // Arrange
        // Act
        var result = LicensePlate.Create(value);

        // Assert
        result.Should().BeSuccess();
        result.Value.Value.Should().Be(value.Replace("-", "").ToUpper());
    }
    
    [Theory, MemberData(nameof(InvalidLicensePlates))]
    public void Create_WhenValueIsInvalid_ShouldReturnMalformed(string value)
    {
        // Arrange
        // Act
        var result = LicensePlate.Create(value);

        // Assert
        result.Should().BeFailure(LicensePlate.Malformed);
    }
}