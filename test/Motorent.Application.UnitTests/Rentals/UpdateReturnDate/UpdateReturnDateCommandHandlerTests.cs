using Motorent.Application.Rentals.Common.Errors;
using Motorent.Application.Rentals.UpdateReturnDate;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.Services;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.TestUtils.Constants;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Rentals.UpdateReturnDate;

[TestSubject(typeof(UpdateReturnDateCommandHandler))]
public sealed class UpdateReturnDateCommandHandlerTests : IAsyncLifetime
{
    private static readonly UpdateReturnDateCommand Command = new()
    {
        RentalId = Constants.Rental.Id.Value,
        // 10 dias após a data de retorno para o plano de locação
        ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(Constants.Rental.Plan.Days + 10))
    };

    private readonly IRentalRepository rentalRepository = A.Fake<IRentalRepository>();
    private readonly IRentalPenaltyService rentalPenaltyService = A.Fake<IRentalPenaltyService>();

    private readonly UpdateReturnDateCommandHandler sut;

    private Rental rental = null!;

    public UpdateReturnDateCommandHandlerTests()
    {
        sut = new UpdateReturnDateCommandHandler(rentalRepository, rentalPenaltyService);
    }

    public Task InitializeAsync()
    {
        var rentalId = new RentalId(Command.RentalId);
        rental = Factories.Rental.Create(rentalId);

        A.CallTo(() => rentalRepository.FindAsync(rentalId, A<CancellationToken>._))
            .Returns(rental);

        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_WhenRentalDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        A.CallTo(() => rentalRepository.FindAsync(new RentalId(Command.RentalId), A<CancellationToken>._))
            .Returns(null as Rental);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeFailure(RentalErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenRentalExists_ShouldChangeReturnDate()
    {
        // Arrange
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
    }

    [Fact]
    public async Task Handle_WhenRentalExists_ShouldUpdateRental()
    {
        // Arrange
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => rentalRepository.UpdateAsync(rental, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenChangeReturnDateFails_ShouldReturnFailure()
    {
        // Arrange
        var pastReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var result = await sut.Handle(Command with
        {
            ReturnDate = pastReturnDate
        }, CancellationToken.None);

        // Assert
        result.Should().BeFailure();
    }
    
    [Fact]
    public async Task Handle_WhenChangeReturnDateFails_ShouldNotUpdateRental()
    {
        // Arrange
        var pastReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        await sut.Handle(Command with
        {
            ReturnDate = pastReturnDate
        }, CancellationToken.None);

        // Assert
        A.CallTo(() => rentalRepository.UpdateAsync(rental, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}