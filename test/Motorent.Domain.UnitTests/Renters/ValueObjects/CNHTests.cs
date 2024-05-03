using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.UnitTests.Renters.ValueObjects;

[TestSubject(typeof(CNH))]
public sealed class CNHTests
{
    private const string Number = "83907030608";
    private static readonly DateOnly ExpirationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
    private static readonly CNHCategory Category = CNHCategory.AB;
    
    public static readonly IEnumerable<object[]> ValidCNHNumbers = new List<object[]>
    {
        new object[] { "83907030608" },
        new object[] { "50542712396" },
        new object[] { "84407903668" },
        new object[] { "26775490609" },
        new object[] { "00351664298" }
    };

    public static readonly IEnumerable<object[]> InvalidCNHNumbers = new List<object[]>
    {
        new object[] { "2677549060" },
        new object[] { "00351864298" },
        new object[] { "11111111111" }
    };
    
    [Fact]
    public void Create_WhenValuesAreValid_ShouldReturnCnh()
    {
        // Arrange
        // Act
        var result = CNH.Create(Number, Category, ExpirationDate);

        // Assert
        result.Should().BeSuccess();
        
        result.Value.Number.Should().Be(Number);
        result.Value.ExpirationDate.Should().Be(ExpirationDate);
        result.Value.Category.Should().Be(Category);
    }

    [Theory, MemberData(nameof(ValidCNHNumbers))]
    public void Create_WhenNumberIsValid_ShouldReturnCnhNumber(string number)
    {
        // Arrange
        // Act
        var result = CNH.Create(number, Category, ExpirationDate);

        // Assert
        result.Should().BeSuccess(number);
    }

    [Theory, MemberData(nameof(InvalidCNHNumbers))]
    public void Create_WhenNumberIsInvalid_ShouldReturnInvalid(string number)
    {
        // Arrange
        // Act
        var result = CNH.Create(number, Category, ExpirationDate);

        // Assert
        result.Should().BeFailure(CNH.Invalid);
    }

    [Fact]
    public void Create_WhenIsExpired_ShouldReturnExpired()
    {
        // Arrange
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var result = CNH.Create(Number, Category, expirationDate);

        // Assert
        result.Should().BeFailure(CNH.Expired);
    }
}