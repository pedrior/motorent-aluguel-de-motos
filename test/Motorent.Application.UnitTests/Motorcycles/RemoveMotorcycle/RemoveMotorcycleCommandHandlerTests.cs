using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Application.Motorcycles.RemoveMotorcycle;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.TestUtils.Constants;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Motorcycles.RemoveMotorcycle;

[TestSubject(typeof(RemoveMotorcycleCommandHandler))]
public sealed class RemoveMotorcycleCommandHandlerTests
{
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();

    private readonly RemoveMotorcycleCommand command = new(Constants.Motorcycle.Id.Value);

    private readonly RemoveMotorcycleCommandHandler sut;

    public RemoveMotorcycleCommandHandlerTests()
    {
        sut = new RemoveMotorcycleCommandHandler(motorcycleRepository);
    }

    [Fact]
    public async Task Handle_WhenMotorcycleExists_ShouldDeleteMotorcycle()
    {
        // Arrange
        var motorcycleId = new MotorcycleId(command.Id);
        var motorcycle = await Factories.Motorcycle.CreateAsync();

        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(motorcycle.Value);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();

        A.CallTo(() => motorcycleRepository.DeleteAsync(motorcycle.Value, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
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