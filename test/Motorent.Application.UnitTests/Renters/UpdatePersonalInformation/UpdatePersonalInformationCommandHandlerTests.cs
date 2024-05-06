using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Renters.UpdatePersonalInformation;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Renters.UpdatePersonalInformation;

[TestSubject(typeof(UpdatePersonalInformationCommandHandler))]
public sealed class UpdatePersonalInformationCommandHandlerTests
{
    private readonly IUserContext userContext = A.Fake<IUserContext>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();

    private readonly UpdatePersonalInformationCommandHandler sut;

    private readonly UpdatePersonalInformationCommand command = new()
    {
        GivenName = "Jane",
        FamilyName = "Doe",
        Birthdate = new DateOnly(2000, 09, 05)
    };

    public UpdatePersonalInformationCommandHandlerTests()
    {
        sut = new UpdatePersonalInformationCommandHandler(userContext, renterRepository);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdatePersonalInformation()
    {
        // Arrange
        var userId = Ulid.NewUlid().ToString();

        A.CallTo(() => userContext.UserId)
            .Returns(userId);

        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;
        
        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);
        
        // Act
        var result = await sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().BeSuccess();
        
        renter.FullName.GivenName.Should().Be(command.GivenName);
        renter.FullName.FamilyName.Should().Be(command.FamilyName);
        renter.Birthdate.Value.Should().Be(command.Birthdate);
        
        A.CallTo(() => renterRepository.UpdateAsync(renter, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task Handle_WhenBirthdateIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        // Act
        var result = await sut.Handle(command with
        {
            Birthdate = DateOnly.FromDateTime(DateTime.Today)
        }, CancellationToken.None);
        
        // Assert
        result.Should().BeFailure();
    }
    
    [Fact]
    public async Task Handle_WhenRenterDoesNotExist_ShouldThrowApplicationException()
    {
        // Arrange
        A.CallTo(() => renterRepository.FindByUserAsync(A<string>._, A<CancellationToken>._))
            .Returns(null as Renter);

        // Act
        var act = () => sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Renter not found for user *");
    }
}