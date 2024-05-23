using Motorent.Application.Rentals.Common.Errors;
using Motorent.Application.Rentals.Common.Mappings;
using Motorent.Application.Rentals.GetRental;
using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Rentals.GetRental;

[TestSubject(typeof(GetRentalQueryHandler))]
public sealed class GetRentalQueryHandlerTests : IAsyncLifetime
{
    private static readonly RentalId RentalId = RentalId.New();

    private static readonly GetRentalQuery Query = new()
    {
        Id = RentalId.Value
    };

    private readonly IRentalRepository rentalRepository = A.Fake<IRentalRepository>();
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();

    private readonly GetRentalQueryHandler sut;

    private Rental rental = null!;
    private Motorcycle motorcycle = null!;

    static GetRentalQueryHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Apply(new RentalMappings());
    }

    public GetRentalQueryHandlerTests()
    {
        sut = new GetRentalQueryHandler(rentalRepository, motorcycleRepository);
    }

    public async Task InitializeAsync()
    {
        motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        rental = Factories.Rental.Create(RentalId, motorcycleId: motorcycle.Id);

        A.CallTo(() => rentalRepository.FindAsync(RentalId, A<CancellationToken>._))
            .Returns(rental);

        A.CallTo(() => motorcycleRepository.FindAsync(rental.MotorcycleId, A<CancellationToken>._))
            .Returns(motorcycle);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnRentalResponse()
    {
        // Arrange
        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.Should().BeSuccess()
            .Which.Value.Should().BeEquivalentTo(new
            {
                Id = rental.Id.ToString(),
                Motorcycle = new RentalMotorcycleResponse
                {
                    Id = motorcycle.Id.ToString(),
                    Model = motorcycle.Model,
                    Year = motorcycle.Year.Value,
                    LicensePlate = motorcycle.LicensePlate.Value
                }
            });
    }

    [Fact]
    public async Task Handle_WhenRentalNotFound_ShouldReturnNotFound()
    {
        // Arrange
        A.CallTo(() => rentalRepository.FindAsync(RentalId, A<CancellationToken>._))
            .Returns(null as Rental);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.Should().BeFailure(RentalErrors.NotFound);

        A.CallTo(motorcycleRepository)
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenMotorcycleNotFound_ShouldThrowApplicationException()
    {
        // Arrange
        A.CallTo(() => motorcycleRepository.FindAsync(rental.MotorcycleId, A<CancellationToken>._))
            .Returns(null as Motorcycle);

        // Act
        var act = () => sut.Handle(Query, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"Motorcycle with id {rental.MotorcycleId} not found.");
    }
}