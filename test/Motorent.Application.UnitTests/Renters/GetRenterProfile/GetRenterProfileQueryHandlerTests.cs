using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Renters.Common.Mappings;
using Motorent.Application.Renters.GetRenterProfile;
using Motorent.Contracts.Renters.Responses;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.TestUtils.Constants;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Renters.GetRenterProfile;

[TestSubject(typeof(GetRenterProfileQueryHandler))]
public sealed class GetRenterProfileQueryHandlerTests
{
    private readonly IUserContext userContext = A.Fake<IUserContext>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();
    private readonly IStorageService storageService = A.Fake<IStorageService>();

    private readonly GetRenterProfileQueryHandler sut;

    private readonly GetRenterProfileQuery query = new();

    public GetRenterProfileQueryHandlerTests()
    {
        TypeAdapterConfig.GlobalSettings.Apply(new RenterMappings());

        sut = new GetRenterProfileQueryHandler(userContext, renterRepository, storageService);

        A.CallTo(() => storageService.GenerateUrlAsync(A<Uri>._, A<int>._))
            .Returns(Constants.Renter.DriverLicenseImage);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnRenterProfileResponse()
    {
        // Arrange
        var userId = Ulid.NewUlid().ToString();

        A.CallTo(() => userContext.UserId)
            .Returns(userId);

        var renter = (await Factories.Renter.CreateAsync()).Value;

        A.CallTo(() => renterRepository.FindByUserAsync(userId, A<CancellationToken>._))
            .Returns(renter);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeOfType<RenterProfileResponse>();
    }

    [Fact]
    public async Task Handle_WhenRenterDoesNotExist_ShouldThrowApplicationException()
    {
        // Arrange
        A.CallTo(() => renterRepository.FindByUserAsync(A<string>._, A<CancellationToken>._))
            .Returns(null as Renter);

        // Act
        var act = () => sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Renter not found for user *");
    }
}