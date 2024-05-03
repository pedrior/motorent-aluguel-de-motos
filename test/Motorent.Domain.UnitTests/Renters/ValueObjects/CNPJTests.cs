using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.UnitTests.Renters.ValueObjects;

[TestSubject(typeof(CNPJ))]
public sealed class CNPJTests
{
    public static readonly IEnumerable<object[]> ValidCNPJs = new List<object[]>
    {
        new object[] { "27137033000134" },
        new object[] { "97877245000133" },
        new object[] { "96.657.097/0001-89" },
        new object[] { "72.302.462/0001-74" }
    };

    public static readonly IEnumerable<object[]> InvalidCNPJs = new List<object[]>
    {
        new object[] { "2713703300013" },
        new object[] { "96.657.09780001-89" },
        new object[] { "74798886000161" }
    };

    [Theory, MemberData(nameof(ValidCNPJs))]
    public void Create_WhenCNPJIsValid_ShouldReturnCnpj(string value)
    {
        // Arrange
        // Act
        var result = CNPJ.Create(value);

        // Assert
        result.Should().BeSuccess(value);
    }
    
    [Theory, MemberData(nameof(InvalidCNPJs))]
    public void Create_WhenCNPJIsInvalid_ShouldReturnInvalid(string value)
    {
        // Arrange
        // Act
        var result = CNPJ.Create(value);

        // Assert
        result.Should().BeFailure(CNPJ.Invalid);
    }
}