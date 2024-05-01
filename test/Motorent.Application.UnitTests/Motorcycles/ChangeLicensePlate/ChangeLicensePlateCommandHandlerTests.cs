using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Application.Motorcycles.UpdateLicensePlate;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Motorcycles.ChangeLicensePlate;

[TestSubject(typeof(ChangeLicensePlateCommandHandler))]
public sealed class ChangeLicensePlateCommandHandlerTests
{
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();
    private readonly ILicensePlateService licensePlateService = A.Fake<ILicensePlateService>();

    private readonly ChangeLicensePlateCommandHandler sut;

    private readonly ChangeLicensePlateCommand command = new()
    {
        Id = Ulid.NewUlid(),
        LicensePlate = "HAC-8Y96"
    };

    public ChangeLicensePlateCommandHandlerTests()
    {
        sut = new ChangeLicensePlateCommandHandler(motorcycleRepository, licensePlateService);
    }

    [Fact]
    public async Task Handle_WhenCalledWithValidCommand_ShouldUpdateMotorcycleAndReturnSuccess()
    {
        // Arrange
        var motorcycleId = new MotorcycleId(command.Id);
        var motorcycle = await Factories.Motorcycle.CreateAsync(id: motorcycleId);
        
        var licensePlate = LicensePlate.Create(command.LicensePlate);
        
        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(motorcycle.Value);
        
        A.CallTo(() => licensePlateService.IsUniqueAsync(licensePlate.Value, A<CancellationToken>._))
            .Returns(true);
        
        // Act
        var result = await sut.Handle(command, CancellationToken.None);
        
        // Assert
        A.CallTo(() => motorcycleRepository.UpdateAsync(motorcycle.Value, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        
        result.Should().BeSuccess();
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

    [Fact]
    public async Task Handle_WhenLicensePlateIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var motorcycleId = new MotorcycleId(command.Id);
        var motorcycle = await Factories.Motorcycle.CreateAsync(id: motorcycleId);
        
        var commandWithInvalidLicensePlate = command with
        {
            LicensePlate = "invalid"
        };
        
        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(motorcycle.Value);
        
        // Act
        var result = await sut.Handle(commandWithInvalidLicensePlate, CancellationToken.None);
        
        // Assert
        result.Should().BeFailure();
    }
}