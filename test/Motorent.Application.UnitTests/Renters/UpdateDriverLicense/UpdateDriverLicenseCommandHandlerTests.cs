using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Renters.UpdateDriverLicense;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Renters.UpdateDriverLicense;

[TestSubject(typeof(UpdateDriverLicenseCommandHandler))]
public sealed class UpdateDriverLicenseCommandHandlerTests
{
    private readonly IUserContext userContext = A.Fake<IUserContext>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();
    private readonly IDriverLicenseService driverLicenseService = A.Fake<IDriverLicenseService>();

    private readonly UpdateDriverLicenseCommandHandler sut;

    private readonly UpdateDriverLicenseCommand command = new()
    {
        Number = "92353762700",
        Category = "ab",
        Expiry = new DateOnly(DateTime.Today.Year + 1, 01, 01)
    };

    private readonly string userId = Ulid.NewUlid().ToString();

    public UpdateDriverLicenseCommandHandlerTests()
    {
        sut = new UpdateDriverLicenseCommandHandler(userContext, renterRepository, driverLicenseService);

        A.CallTo(() => userContext.UserId)
            .Returns(userId);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldChangeDriverLicense()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => driverLicenseService.IsUniqueAsync(A<DriverLicense>._, A<CancellationToken>._))
            .Returns(true);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        renter.DriverLicense.Number.Should().Be(command.Number);
        renter.DriverLicense.Category.Name.Should().BeEquivalentTo(command.Category);
        renter.DriverLicense.Expiry.Should().Be(command.Expiry);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateRenter()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => driverLicenseService.IsUniqueAsync(A<DriverLicense>._, A<CancellationToken>._))
            .Returns(true);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => renterRepository.UpdateAsync(renter, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenDriverLicenseIsInvalid_ShouldReturnFailure()
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
        
        A.CallTo(() => driverLicenseService.IsUniqueAsync(A<DriverLicense>._, A<CancellationToken>._))
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
    public async Task Handle_WhenDriverLicenseIsNotUnique_ShouldReturnFailure()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        A.CallTo(() => driverLicenseService.IsUniqueAsync(A<DriverLicense>._, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFailure();
        
        A.CallTo(() => renterRepository.UpdateAsync(A<Renter>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}