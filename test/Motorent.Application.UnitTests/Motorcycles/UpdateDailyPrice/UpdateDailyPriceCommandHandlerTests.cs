using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Application.Motorcycles.UpdateDailyPrice;
using Motorent.Domain.Common.ValueObjects;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Motorcycles.UpdateDailyPrice;

[TestSubject(typeof(UpdateDailyPriceCommandHandler))]
public sealed class UpdateDailyPriceCommandHandlerTests
{
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();

    private readonly UpdateDailyPriceCommandHandler sut;

    private readonly UpdateDailyPriceCommand command = new()
    {
        Id = Ulid.NewUlid(),
        DailyPrice = 60
    };

    public UpdateDailyPriceCommandHandlerTests()
    {
        sut = new UpdateDailyPriceCommandHandler(motorcycleRepository);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateDailyPrice()
    {
        // Arrange
        var motorcycleId = new MotorcycleId(command.Id);
        var motorcycle = (await Factories.Motorcycle.CreateAsync(id: motorcycleId)).Value;
        
        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(motorcycle);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        
        motorcycle.DailyPrice.Should().Be(Money.Create(command.DailyPrice).Value);

        A.CallTo(() => motorcycleRepository.UpdateAsync(motorcycle, A<CancellationToken>._))
            .MustHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenDailyPriceIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        const decimal dailyPrice = -1;

        // Act
        var result = await sut.Handle(command with { DailyPrice = dailyPrice }, CancellationToken.None);

        // Assert
        result.Should().BeFailure();

        A.CallTo(() => motorcycleRepository.FindAsync(A<MotorcycleId>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var motorcycleId = new MotorcycleId(command.Id);
        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(null as Motorcycle);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFailure(MotorcycleErrors.NotFound);

        A.CallTo(() => motorcycleRepository.UpdateAsync(A<Motorcycle>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}