using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Domain.UnitTests.Rentals.ValueObjects;

[TestSubject(typeof(Period))]
public sealed class PeriodTests
{
    [Fact]
    public void New_WhenEndDateIsInThePast_ShouldThrowArgumentException()
    {
        // Arrange
        var end = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        // Act
        var act = () => new Period(end);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("End date must be in the future. (Parameter 'end')");
    }
    
    [Fact]
    public void New_WhenCalled_ShouldCreatePeriodWithStartDateOneDayAfterTodayDate()
    {
        // Arrange
        var end = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(15);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        // Act
        var period = new Period(end);
        
        // Assert
        period.Should().NotBeNull();
        period.Start.Should().Be(today.AddDays(1));
    }
}