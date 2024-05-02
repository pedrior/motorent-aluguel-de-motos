using Motorent.Application.Common.Mappings;
using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Application.Motorcycles.Common.Mappings;
using Motorent.Application.Motorcycles.GetMotorcycle;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Motorcycles.GetMotorcycle;

[TestSubject(typeof(GetMotorcycleQueryHandler))]
public sealed class GetMotorcycleQueryHandlerTests
{
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();

    private readonly GetMotorcycleQueryHandler sut;

    public GetMotorcycleQueryHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Apply(new CommonMappings());
        TypeAdapterConfig.GlobalSettings.Apply(new MotorcycleMappings());

        sut = new GetMotorcycleQueryHandler(motorcycleRepository);
    }

    [Fact]
    public async Task Handle_WhenMotorcycleExistsById_ShouldReturnMotorcycleResponse()
    {
        // Arrange
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        var motorcycleId = motorcycle.Id;

        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(motorcycle);

        var query = new GetMotorcycleQuery(motorcycleId.Value.ToString());

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeOfType<MotorcycleResponse>();
    }

    [Fact]
    public async Task Handle_WhenMotorcycleExistsByLicensePlate_ShouldReturnMotorcycleResponse()
    {
        // Arrange
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        var licensePlate = motorcycle.LicensePlate;

        A.CallTo(() => motorcycleRepository.FindByLicensePlateAsync(licensePlate, A<CancellationToken>._))
            .Returns(motorcycle);

        var query = new GetMotorcycleQuery(licensePlate.Value);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeOfType<MotorcycleResponse>();
    }

    [Fact]
    public async Task Handle_WhenMotorcycleDoesNotExistById_ShouldReturnNotFound()
    {
        // Arrange
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        var motorcycleId = motorcycle.Id;

        A.CallTo(() => motorcycleRepository.FindAsync(motorcycleId, A<CancellationToken>._))
            .Returns(null as Motorcycle);

        var query = new GetMotorcycleQuery(motorcycleId.Value.ToString());

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeFailure(MotorcycleErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_WhenMotorcycleDoesNotExistByLicensePlate_ShouldReturnNotFound()
    {
        // Arrange
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        var licensePlate = motorcycle.LicensePlate;

        A.CallTo(() => motorcycleRepository.FindByLicensePlateAsync(licensePlate, A<CancellationToken>._))
            .Returns(null as Motorcycle);

        var query = new GetMotorcycleQuery(licensePlate.Value);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeFailure(MotorcycleErrors.NotFound);
    }
}