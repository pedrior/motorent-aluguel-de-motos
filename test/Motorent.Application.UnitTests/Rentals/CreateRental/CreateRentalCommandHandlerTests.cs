using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Mappings;
using Motorent.Application.Rentals.Common.Errors;
using Motorent.Application.Rentals.Common.Mappings;
using Motorent.Application.Rentals.CreateRental;
using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.Services;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.TestUtils.Constants;
using Motorent.TestUtils.Factories;
using ResultExtensions;

namespace Motorent.Application.UnitTests.Rentals.CreateRental;

[TestSubject(typeof(CreateRentalCommandHandler))]
public sealed class CreateRentalCommandHandlerTests : IAsyncLifetime
{
    private static readonly string UserId = Ulid.NewUlid().ToString();

    private static readonly CreateRentalCommand Command = new()
    {
        Plan = Constants.Rental.Plan.Name,
        MotorcycleId = Constants.Motorcycle.Id.Value
    };

    private readonly IUserContext userContext = A.Fake<IUserContext>();
    private readonly IRentalFactory rentalFactory = A.Fake<IRentalFactory>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();
    private readonly IMotorcycleRepository motorcycleRepository = A.Fake<IMotorcycleRepository>();
    private readonly IRentalRepository rentalRepository = A.Fake<IRentalRepository>();

    private readonly CreateRentalCommandHandler sut;

    private Renter renter = null!;
    private Motorcycle motorcycle = null!;

    static CreateRentalCommandHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Apply(new CommonMappings());
        TypeAdapterConfig.GlobalSettings.Apply(new RentalMappings());
    }
    
    public CreateRentalCommandHandlerTests()
    {
        sut = new CreateRentalCommandHandler(
            userContext,
            rentalFactory,
            renterRepository,
            motorcycleRepository,
            rentalRepository);

        A.CallTo(() => userContext.UserId)
            .Returns(UserId);

        A.CallTo(() => motorcycleRepository.ExistsAsync(
                new MotorcycleId(Command.MotorcycleId),
                A<CancellationToken>._))
            .Returns(true);
    }

    public async Task InitializeAsync()
    {
        renter = (await Factories.Renter.CreateAsync(userId: UserId)).Value;
        motorcycle = (await Factories.Motorcycle.CreateAsync(
            id: new MotorcycleId(Command.MotorcycleId))).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(UserId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => motorcycleRepository.FindAsync(
                new MotorcycleId(Command.MotorcycleId),
                A<CancellationToken>._))
            .Returns(motorcycle);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnRentalResponse()
    {
        // Arrange
        var rental = Factories.Rental.Create(renterId: renter.Id, motorcycleId: motorcycle.Id);

        A.CallTo(() => rentalFactory.Create(
                renter,
                A<RentalId>._,
                new MotorcycleId(Command.MotorcycleId),
                RentalPlan.FromName(Command.Plan, true)))
            .Returns(rental);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeOfType<RentalResponse>().And.NotBeNull();
        result.Value.Should().BeEquivalentTo(new
        {
            Motorcycle = new RentalMotorcycleResponse
            {
                Id = motorcycle.Id.ToString(),
                Model = motorcycle.Model,
                Year = motorcycle.Year.Value,
                LicensePlate = motorcycle.LicensePlate.ToString()
            }
        });
    }
    
    [Fact]
    public async Task Handle_WhenCalled_ShouldAddRentalToRepository()
    {
        // Arrange
        var rental = Factories.Rental.Create(renterId: renter.Id, motorcycleId: motorcycle.Id);

        A.CallTo(() => rentalFactory.Create(
                renter,
                A<RentalId>._,
                new MotorcycleId(Command.MotorcycleId),
                RentalPlan.FromName(Command.Plan, true)))
            .Returns(rental);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => rentalRepository.AddAsync(rental, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenRenterDoesNotExist_ShouldThrowApplicationException()
    {
        // Arrange
        A.CallTo(() => renterRepository.FindByUserAsync(UserId, A<CancellationToken>._))
            .Returns(null as Renter);

        // Act
        var act = () => sut.Handle(Command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"Renter not found for user {UserId}.");

        A.CallTo(rentalFactory)
            .MustNotHaveHappened();

        A.CallTo(motorcycleRepository)
            .MustNotHaveHappened();

        A.CallTo(rentalRepository)
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenMotorcycleDoesNotExist_ShouldReturnMotorcycleNotFound()
    {
        // Arrange
        var motorcycleId = new MotorcycleId(Command.MotorcycleId);

        A.CallTo(() => motorcycleRepository.ExistsAsync(motorcycleId, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeFailure(RentalErrors.MotorcycleNotFound(motorcycleId));

        A.CallTo(rentalFactory)
            .MustNotHaveHappened();

        A.CallTo(rentalRepository)
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRentalFactoryFails_ShouldReturnFailure()
    {
        // Arrange
        var error = Error.Failure("test error");

        A.CallTo(() => rentalFactory.Create(
                renter,
                A<RentalId>._,
                new MotorcycleId(Command.MotorcycleId),
                RentalPlan.FromName(Command.Plan, true)))
            .Returns(error);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeFailure(error);

        A.CallTo(rentalRepository)
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenMotorcycleDoesNotExistAfterRentalCreation_ShouldThrowApplicationException()
    {
        // Arrange
        var rental = Factories.Rental.Create(renterId: renter.Id, motorcycleId: motorcycle.Id);

        A.CallTo(() => rentalFactory.Create(
                renter,
                A<RentalId>._,
                new MotorcycleId(Command.MotorcycleId),
                RentalPlan.FromName(Command.Plan, true)))
            .Returns(rental);

        A.CallTo(() => motorcycleRepository.FindAsync(motorcycle.Id, A<CancellationToken>._))
            .Returns(null as Motorcycle);

        // Act
        var act = () => sut.Handle(Command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<ApplicationException>()
            .WithMessage($"Motorcycle not found for rental {rental.Id}.");
    }
}