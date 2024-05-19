using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Renters.UploadCNHImage;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Renters.UploadCNHImage;

[TestSubject(typeof(UploadCNHImageCommandHandler))]
public sealed class UploadCNHImageCommandHandlerTests
{
    private readonly IUserContext userContext = A.Fake<IUserContext>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();
    private readonly IStorageService storageService = A.Fake<IStorageService>();

    private readonly UploadCNHImageCommandHandler sut;

    private readonly UploadCNHImageCommand command = new()
    {
        Image = A.Fake<IFile>()
    };

    private readonly string userId = Ulid.NewUlid().ToString();

    public UploadCNHImageCommandHandlerTests()
    {
        sut = new UploadCNHImageCommandHandler(userContext, renterRepository, storageService);

        A.CallTo(() => userContext.UserId)
            .Returns(userId);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldSendCNHValidationImages()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        // Act
        var result = await sut.Handle(command, default);

        // Assert
        result.Should().BeSuccess();
        renter.CNHImageUrl.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldUpdateRenter()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        // Act
        await sut.Handle(command, default);

        // Assert
        A.CallTo(() => renterRepository.UpdateAsync(renter, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
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