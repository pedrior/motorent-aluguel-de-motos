using Motorent.Application.Auth.Login;
using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Contracts.Auth.Responses;

namespace Motorent.Application.UnitTests.Auth.Login;

[TestSubject(typeof(LoginCommandHandler))]
public sealed class LoginCommandHandlerTests
{
    private readonly IUserService userService = A.Fake<IUserService>();
    private readonly ISecurityTokenProvider securityTokenProvider = A.Fake<ISecurityTokenProvider>();

    private readonly LoginCommandHandler sut;

    private readonly LoginCommand command = new()
    {
        Email = "john@doe.com",
        Password = "JohnDoe123"
    };

    public LoginCommandHandlerTests()
    {
        sut = new LoginCommandHandler(userService, securityTokenProvider);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnTokenResponse()
    {
        // Arrange
        const string userId = "user-id";
        var securityToken = new SecurityToken("access-token", 3600);
        
        A.CallTo(() => userService.CheckPasswordAsync(command.Email, command.Password, A<CancellationToken>._))
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
    public async Task Handle_WhenCheckPasswordFails_ShouldReturnFailure()
    {
        // Arrange
        var error = UserErrors.InvalidCredentials;
        A.CallTo(() => userService.CheckPasswordAsync(A<string>._, A<string>._, A<CancellationToken>._))
            .Returns(error);
        
        // Act
        var result = await sut.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().BeFailure(error);
    }
    
    [Fact]
    public async Task Handle_WhenCheckPasswordFails_ShouldNotGenerateSecurityToken()
    {
        // Arrange
        A.CallTo(() => userService.CheckPasswordAsync(A<string>._, A<string>._, A<CancellationToken>._))
            .Returns(UserErrors.InvalidCredentials);
        
        // Act
        await sut.Handle(command, CancellationToken.None);
        
        // Assert
        A.CallTo(() => securityTokenProvider.GenerateSecurityTokenAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}