using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.UnitTests.Motorcycles.ValueObjects;

[TestSubject(typeof(Year))]
public sealed class YearTests
{
    [Fact]
    public void Create_WhenValueIsNewerThan5Years_ShouldReturnYear()
    {
        // Arrange
        var value = DateTime.UtcNow.Year;

        // Act
        var result = Year.Create(value);

        // Assert
        result.Should().BeSuccess();
        result.Value.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WhenValueIs5YearsOld_ShouldReturnYear()
    {
        // Arrange
        var value = DateTime.UtcNow.Year - 5;

        // Act
        var result = Year.Create(value);

        // Assert
        result.Should().BeSuccess();
        result.Value.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WhenValueIsOlderThan5Years_ShouldReturnToolOld()
    {
        // Arrange
        const int value = 2010;

        // Act
        var result = Year.Create(value);

        // Assert
        result.Should().BeFailure(Year.ToolOld);
    }
}