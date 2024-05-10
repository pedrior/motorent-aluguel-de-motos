using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Renters.UpdateCNH;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Renters.UpdateCNH;

[TestSubject(typeof(UpdateCNHCommandHandler))]
public sealed class UpdateCNHCommandHandlerTests
{
    private readonly IUserContext userContext = A.Fake<IUserContext>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();
    private readonly ICNHService cnhService = A.Fake<ICNHService>();

    private readonly UpdateCNHCommandHandler sut;

    private readonly UpdateCNHCommand command = new()
    {
        Number = "92353762700",
        Category = "ab",
        ExpDate = new DateOnly(DateTime.Today.Year + 1, 01, 01)
    };

    private readonly string userId = Ulid.NewUlid().ToString();

    public UpdateCNHCommandHandlerTests()
    {
        sut = new UpdateCNHCommandHandler(userContext, renterRepository, cnhService);

        A.CallTo(() => userContext.UserId)
            .Returns(userId);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldChangeCNH()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => cnhService.IsUniqueAsync(A<CNH>._, A<CancellationToken>._))
            .Returns(true);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        renter.CNH.Number.Should().Be(command.Number);
        renter.CNH.Category.Name.Should().BeEquivalentTo(command.Category);
        renter.CNH.ExpirationDate.Should().Be(command.ExpDate);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateRenter()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => cnhService.IsUniqueAsync(A<CNH>._, A<CancellationToken>._))
            .Returns(true);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => renterRepository.UpdateAsync(renter, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenCNHIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var newCommand = command with
        {
            Number = "1234567890"
        };
        
        // Act
        var result = await sut.Handle(newCommand, CancellationToken.None);
        
        // Assert
        result.Should().BeFailure();
        
        A.CallTo(() => cnhService.IsUniqueAsync(A<CNH>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        
        A.CallTo(() => renterRepository.FindByUserAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        
        A.CallTo(() => renterRepository.UpdateAsync(A<Renter>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
    
    [Fact]
    public async Task Handle_WhenRenterIsNotFound_ShouldThrowApplicationException()
    {
        // Arrange
        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(null as Renter);

        // Act
        Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage($"Renter not found for user {userId}");
    }
    
    [Fact]
    public async Task Handle_WhenCNHIsNotUnique_ShouldReturnFailure()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => cnhService.IsUniqueAsync(A<CNH>._, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFailure();
        
        A.CallTo(() => renterRepository.UpdateAsync(A<Renter>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}