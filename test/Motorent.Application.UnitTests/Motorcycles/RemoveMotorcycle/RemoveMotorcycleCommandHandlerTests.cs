using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Application.Motorcycles.RemoveMotorcycle;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Repository;
using Motorent.TestUtils.Constants;
using Motorent.TestUtils.Factories;
using ResultExtensions;

namespace Motorent.Application.UnitTests.Motorcycles.RemoveMotorcycle;

[TestSubject(typeof(RemoveMotorcycleCommandHandler))]
public sealed class RemoveMotorcycleCommandHandlerTests
{
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();
    private readonly IRentalRepository rentalRepository = A.Fake<IRentalRepository>();
    private readonly IMotorcycleDeletionService motorcycleDeletionService = A.Fake<IMotorcycleDeletionService>();

    private readonly RemoveMotorcycleCommand command = new(Constants.Motorcycle.Id.Value);

    private readonly RemoveMotorcycleCommandHandler sut;

    public RemoveMotorcycleCommandHandlerTests()
    {
        sut = new RemoveMotorcycleCommandHandler(
            motorcycleRepository,
            rentalRepository,
            motorcycleDeletionService);
    }

    [Fact]
    public async Task Handle_WhenMotorcycleExists_ShouldDeleteMotorcycle()
    {
        // Arrange
        var motorcycleId = new MotorcycleId(command.Id);
        var motorcycle = await Factories.Motorcycle.CreateAsync();
        var rentals = new List<Rental>().AsReadOnly();

        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(motorcycle.Value);

        A.CallTo(() => rentalRepository.ListRentalsByMotorcycleAsync(motorcycle.Value.Id, A<CancellationToken>._))
            .Returns(rentals);

        A.CallTo(() => motorcycleDeletionService.Delete(motorcycle.Value, rentals))
            .Returns(Success.Value);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();

        A.CallTo(() => motorcycleRepository.UpdateAsync(motorcycle.Value, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task Handle_WhenDeletionFailed_ShouldNotUpdateMotorcycle()
    {
        // Arrange
        var motorcycleId = new MotorcycleId(command.Id);
        var motorcycle = await Factories.Motorcycle.CreateAsync();
        var rentals = new List<Rental>().AsReadOnly();

        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(motorcycle.Value);

        A.CallTo(() => rentalRepository.ListRentalsByMotorcycleAsync(motorcycle.Value.Id, A<CancellationToken>._))
            .Returns(rentals);

        A.CallTo(() => motorcycleDeletionService.Delete(motorcycle.Value, rentals))
            .Returns(Error.Failure());

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFailure();

        A.CallTo(() => motorcycleRepository.UpdateAsync(motorcycle.Value, A<CancellationToken>._))
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
    }
}