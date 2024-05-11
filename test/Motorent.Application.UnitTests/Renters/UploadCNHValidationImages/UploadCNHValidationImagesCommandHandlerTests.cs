using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Renters.UploadCNHValidationImages;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Renters.UploadCNHValidationImages;

[TestSubject(typeof(UploadCNHValidationImagesCommandHandler))]
public sealed class UploadCNHValidationImagesCommandHandlerTests
{
    private readonly IUserContext userContext = A.Fake<IUserContext>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();
    private readonly IStorageService storageService = A.Fake<IStorageService>();

    private readonly UploadCNHValidationImagesCommandHandler sut;

    private readonly UploadCNHValidationImagesCommand command = new()
    {
        FrontImage = A.Fake<IFile>(),
        BackImage = A.Fake<IFile>()
    };

    private readonly string userId = Ulid.NewUlid().ToString();

    public UploadCNHValidationImagesCommandHandlerTests()
    {
        sut = new UploadCNHValidationImagesCommandHandler(userContext, renterRepository, storageService);

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
        renter.CNHValidationImages.Should().NotBeNull();
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