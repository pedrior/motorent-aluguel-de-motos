using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Rentals.Common.Mappings;
using Motorent.Application.Rentals.ListRentals;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Rentals.Responses;
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

    private readonly ListRentalsQuery query = new()
    {
        Page = 1,
        Limit = 10
    };

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

        A.CallTo(() => renterRepository.FindByUserAsync(userContext.UserId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => rentalRepository.CountRentalsByRenterAsync(renter.Id, A<CancellationToken>._))
            .Returns(1);

        A.CallTo(() => rentalRepository.ListRentalsByRenterAsync(
                renter.Id, A<int>._, A<int>._, A<CancellationToken>._))
            .Returns([rental]);

        A.CallTo(() => motorcycleRepository.FindAsync(rental.MotorcycleId, A<CancellationToken>._))
            .Returns(motorcycle);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_WhenCalled_ShouldRentalSummaryPageResponse()
    {
        // Arrange
        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeEquivalentTo(new
        {
            query.Page,
            query.Limit,
            TotalItems = 1,
            TotalPages = 1,
            Items = new[]
            {
                rental.Adapt<RentalSummaryResponse>() with
                {
                    Motorcycle = new RentalMotorcycleResponse
                    {
                        Id = motorcycle.Id.ToString(),
                        Model = motorcycle.Model,
                        Year = motorcycle.Year.Value,
                        LicensePlate = motorcycle.LicensePlate.Value
                    } 
                }
            }
        });
    }

    [Fact]
    public async Task Handle_WhenRenterNotFound_ShouldThrowApplicationException()
    {
        // Arrange
        A.CallTo(() => renterRepository.FindByUserAsync(userContext.UserId, A<CancellationToken>._))
            .Returns(null as Renter);

        // Act
        var act = () => sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"Renter not found for user {userId}.");

        A.CallTo(rentalRepository)
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenNoRentalsFound_ShouldReturnEmptyPageResponse()
    {
        // Arrange
        A.CallTo(() => rentalRepository.CountRentalsByRenterAsync(renter.Id, A<CancellationToken>._))
            .Returns(0);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSuccess()
            .Which.Value.Should().BeEquivalentTo(new PageResponse<RentalSummaryResponse>
            {
                Page = query.Page,
                Limit = query.Limit,
                TotalItems = 0,
                TotalPages = 0,
                Items = []
            });

        A.CallTo(() => rentalRepository.ListRentalsByRenterAsync(
                renter.Id, A<int>._, A<int>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
    
    [Fact]
    public async Task Handle_WhenMotorcycleNotFound_ShouldThrowApplicationException()
    {
        // Arrange
        A.CallTo(() => motorcycleRepository.FindAsync(rental.MotorcycleId, A<CancellationToken>._))
            .Returns(null as Motorcycle);

        // Act
        var act = () => sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"Motorcycle with id {rental.MotorcycleId} not found.");
    }
}