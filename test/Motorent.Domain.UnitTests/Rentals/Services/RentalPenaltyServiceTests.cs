using Motorent.Domain.Rentals.Services;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Domain.UnitTests.Rentals.Services;

[TestSubject(typeof(RentalPenaltyService))]
public sealed class RentalPenaltyServiceTests
{
    private readonly RentalPenaltyService sut = new();
    
    [Fact]
    public void Calculate_WhenCurrentReturnDateIsEqualToNewReturnDate_ShouldReturnZero()
    {
        // Arrange
        var currentReturnDate = new DateOnly(2022, 1, 1);
        var newReturnDate = new DateOnly(2022, 1, 1);

        // Act
        var result = sut.Calculate(currentReturnDate, newReturnDate);

        // Assert
        result.Should().Be(Money.Zero);
    }
    
    [Fact]
    public void Calculate_WhenPostponing_ShouldReturnPostponementPenalty()
    {
        // Arrange
        var currentReturnDate = new DateOnly(2022, 1, 1);
        var newReturnDate = new DateOnly(2022, 1, 3); // 2 days

        // Act
        var result = sut.Calculate(currentReturnDate, newReturnDate);

        // Assert
        result.Should().Be(new Money(100.0m));
    }
    
    [Fact]
    public void Calculate_WhenAdvancing_ShouldReturnAdvancePenalty()
    {
        // Arrange
        var currentReturnDate = new DateOnly(2022, 1, 3);
        var newReturnDate = new DateOnly(2022, 1, 1); // 2 days

        // Act
        var result = sut.Calculate(currentReturnDate, newReturnDate);

        // Assert
        result.Should().Be(new Money(0.8m));
    }
}