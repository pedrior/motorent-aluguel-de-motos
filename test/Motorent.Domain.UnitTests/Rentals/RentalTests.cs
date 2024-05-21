using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Enums;

namespace Motorent.Domain.UnitTests.Rentals;

[TestSubject(typeof(Rental))]
public sealed class RentalTests
{
    [Theory]
    [MemberData(nameof(GetRentalPlansTestData))]
    public void Create_WhenCalled_ShouldCreateRentalWithPeriodAndReturnDateBasedOnPlan(RentalPlan plan)
    {
        // Arrange
        // Act
        var rental = Rental.Create(
            Constants.Rental.Id,
            Constants.Renter.Id,
            Constants.Motorcycle.Id,
            plan);
        
        // Assert
        rental.Should().NotBeNull();
        rental.Period.Should().NotBeNull();
        rental.Period.Days.Should().Be(plan.Days);
        rental.ReturnDate.Should().Be(rental.Period.End);
    } 
    
    public static IEnumerable<object[]> GetRentalPlansTestData() => 
        RentalPlan.List.Select(p => new object[] { p });
}