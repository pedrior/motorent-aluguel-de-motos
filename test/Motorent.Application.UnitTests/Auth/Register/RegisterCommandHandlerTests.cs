using Motorent.Application.Auth.Register;
using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Contracts.Auth.Responses;

namespace Motorent.Application.UnitTests.Auth.Register;

[TestSubject(typeof(RegisterCommandHandler))]
public sealed class RegisterCommandHandlerTests
{
    private readonly IUserService userService = A.Fake<IUserService>();
    private readonly ISecurityTokenProvider securityTokenProvider = A.Fake<ISecurityTokenProvider>();

    private readonly RegisterCommandHandler sut;

    private readonly RegisterCommand command = new()
    {
        Email = "john@doe.com",
        Password = "JohnDoe123",
        GivenName = "John",
        FamilyName = "Doe",
        Birthdate = new DateOnly(2000, 09, 05)
    };

    public RegisterCommandHandlerTests()
    {
        sut = new RegisterCommandHandler(userService, securityTokenProvider);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnTokenResponse()
    {
        // Arrange
        const string userId = "user-id";
        var securityToken = new SecurityToken("access-token", 3600);

        A.CallTo(() => userService.CreateUserAsync(
                command.Email,
                command.Password,
                new[] { UserRoles.Renter },
                new Dictionary<string, string>
                {
                    { UserClaimTypes.GivenName, command.GivenName },
                    { UserClaimTypes.FamilyName, command.FamilyName },
                    { UserClaimTypes.Birthdate, command.Birthdate.ToString("yyyy-MM-dd") }
                },
                A<CancellationToken>._))
            .Returns(userId);

        A.CallTo(() => securityTokenProvider.GenerateSecurityTokenAsync(userId, A<CancellationToken>._))
            .Returns(securityToken);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess()
            .Which.Value.Should().BeOfType<TokenResponse>();
    }

    [Fact]
    public async Task Handle_WhenCreateUserFails_ShouldReturnFailure()
    {
        // Arrange
        var error = UserErrors.DuplicateEmail;

        A.CallTo(() => userService.CreateUserAsync(
                A<string>._,
                A<string>._,
                A<string[]>._,
                A<Dictionary<string, string>>._,
                A<CancellationToken>._))
            .Returns(error);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFailure(error);
    }

    [Fact]
    public async Task Handle_WhenCreateUserFails_ShouldNotGenerateSecurityToken()
    {
        // Arrange
        A.CallTo(() => userService.CreateUserAsync(
                A<string>._,
                A<string>._,
                A<string[]>._,
                A<Dictionary<string, string>>._,
                A<CancellationToken>._))
            .Returns(UserErrors.DuplicateEmail);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => securityTokenProvider.GenerateSecurityTokenAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}