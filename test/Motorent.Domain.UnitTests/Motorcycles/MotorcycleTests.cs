using Motorent.Domain.Common.ValueObjects;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Errors;
using Motorent.Domain.Motorcycles.Services;

namespace Motorent.Domain.UnitTests.Motorcycles;

[TestSubject(typeof(Motorcycle))]
public sealed class MotorcycleTests
{
    [Fact]
    public async Task CreateAsync_WhenValuesAreValid_ShouldCreateMotorcycle()
    {
        // Arrange
        // Act
        var result = await Factories.Motorcycle.CreateAsync();

        // Assert
        result.Should().BeSuccess();
    }

    [Fact]
    public async Task CreateAsync_WhenLicensePlateIsNotUnique_ShouldReturnLicensePlateNotUnique()
    {
        // Arrange
        var licensePlate = Constants.Motorcycle.LicensePlate;
        var licensePlateService = A.Fake<ILicensePlateService>();
        A.CallTo(() => licensePlateService.IsUniqueAsync(licensePlate, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await Factories.Motorcycle.CreateAsync(
            licensePlate: licensePlate,
            licensePlateService: licensePlateService);

        // Assert
        result.Should().BeFailure(MotorcycleErrors.LicensePlateNotUnique(licensePlate));
    }

    [Fact]
    public async Task UpdateDailyPrice_ShouldUpdateDailyPrice()
    {
        // Arrange
        var newDailyPrice = Money.Create(35.50m).Value;
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;

        // Act
        motorcycle.UpdateDailyPrice(newDailyPrice);

        // Assert
        motorcycle.DailyPrice.Should().Be(newDailyPrice);
    }

    [Fact]
    public async Task UpdateLicensePlateAsync_WhenLicensePlateIsUnique_ShouldUpdateLicensePlate()
    {
        // Arrange
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        var licensePlate = Constants.Motorcycle.LicensePlate;
        var licensePlateService = A.Fake<ILicensePlateService>();

        A.CallTo(() => licensePlateService.IsUniqueAsync(licensePlate, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await motorcycle.UpdateLicensePlateAsync(
            licensePlate: licensePlate,
            licensePlateService: licensePlateService);

        // Assert
        result.Should().BeSuccess();
        motorcycle.LicensePlate.Should().Be(licensePlate);
    }

    [Fact]
    public async Task UpdateLicensePlateAsync_WhenLicensePlateIsNotUnique_ShouldReturnLicensePlateNotUnique()
    {
        // Arrange
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        var licensePlate = Constants.Motorcycle.LicensePlate;
        var licensePlateService = A.Fake<ILicensePlateService>();

        A.CallTo(() => licensePlateService.IsUniqueAsync(licensePlate, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await motorcycle.UpdateLicensePlateAsync(
            licensePlate: licensePlate,
            licensePlateService: licensePlateService);

        // Assert
        result.Should().BeFailure(MotorcycleErrors.LicensePlateNotUnique(licensePlate));
    }
}