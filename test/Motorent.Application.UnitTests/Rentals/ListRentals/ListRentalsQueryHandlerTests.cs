using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Rentals.Common.Mappings;
using Motorent.Application.Rentals.ListRentals;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Rentals.ListRentals;

[TestSubject(typeof(ListRentalsQueryHandler))]
public sealed class ListRentalsQueryHandlerTests : IAsyncLifetime
{
    private readonly IUserContext userContext = A.Fake<IUserContext>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();
    private readonly IRentalRepository rentalRepository = A.Fake<IRentalRepository>();
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();

    private readonly ListRentalsQueryHandler sut;

    private readonly ListRentalsQuery query = new();

    private readonly string userId = Ulid.NewUlid().ToString();

    private Renter renter = null!;
    private Rental rental = null!;
    private Motorcycle motorcycle = null!;

    static ListRentalsQueryHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Apply(new RentalMappings());
    }

    public ListRentalsQueryHandlerTests()
    {
        sut = new ListRentalsQueryHandler(
            userContext,
            renterRepository,
            rentalRepository,
            motorcycleRepository);
    }

    public async Task InitializeAsync()
    {
        motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        renter = (await Factories.Renter.CreateAsync()).Value;
        rental = Factories.Rental.Create(renterId: renter.Id, motorcycleId: motorcycle.Id);

        A.CallTo(() => userContext.UserId)
            .Returns(userId);
        
        A.CallTo(() => rentalRepository.FindAsync(rental.Id, A<CancellationToken>._))
            .Returns(rental);

        A.CallTo(() => renterRepository.FindByUserAsync(userContext.UserId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => motorcycleRepository.FindAsync(rental.MotorcycleId, A<CancellationToken>._))
            .Returns(motorcycle);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_WhenRenterIsNotFound_ShouldThrowApplicationException()
    {
        // Arrange
        A.CallTo(() => renterRepository.FindByUserAsync(userContext.UserId, A<CancellationToken>._))
            .Returns(null as Renter);

        // Act
        var act = () => sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"Renter not found for user {userId}.");
    }
    
    [Fact]
    public async Task Handle_WhenRenterHasNoRentals_ShouldReturnEmptyList()
    {
        // Arrange
        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeEmpty();
        
        A.CallTo(rentalRepository).MustNotHaveHappened();
        A.CallTo(motorcycleRepository).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task Handle_WhenRenterHasRentals_ShouldReturnListOfRentalSummaryResponses()
    {
        // Arrange
        renter.AddRental(rental);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().HaveCount(1);
        result.Value.First().Motorcycle.Id.Should().Be(motorcycle.Id.ToString());
        result.Value.First().Motorcycle.Model.Should().Be(motorcycle.Model);
        result.Value.First().Motorcycle.Year.Should().Be(motorcycle.Year.Value);
        result.Value.First().Motorcycle.LicensePlate.Should().Be(motorcycle.LicensePlate.Value);
    }
    
    [Fact]
    public async Task Handle_WhenRentaIsNotFound_ShouldThrowApplicationException()
    {
        // Arrange
        renter.AddRental(rental);
        
        A.CallTo(() => rentalRepository.FindAsync(rental.Id, A<CancellationToken>._))
            .Returns(null as Rental);

        // Act
        var act = () => sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"Rental with ID '{rental.Id}' not found.");
    }
    
    [Fact]
    public async Task Handle_WhenMotorcycleIsNotFound_ShouldThrowApplicationException()
    {
        // Arrange
        renter.AddRental(rental);
        
        A.CallTo(() => motorcycleRepository.FindAsync(rental.MotorcycleId, A<CancellationToken>._))
            .Returns(null as Motorcycle);

        // Act
        var act = () => sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"Motorcycle with ID '{rental.MotorcycleId}' not found.");
    }
}