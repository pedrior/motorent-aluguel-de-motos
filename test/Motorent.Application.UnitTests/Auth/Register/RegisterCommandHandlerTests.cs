using Motorent.Application.Auth.Register;
using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Contracts.Auth.Responses;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.TestUtils.Factories;

namespace Motorent.Application.UnitTests.Auth.Register;

[TestSubject(typeof(RegisterCommandHandler))]
public sealed class RegisterCommandHandlerTests
{
    private readonly IUserService userService = A.Fake<IUserService>();
    private readonly ISecurityTokenProvider securityTokenProvider = A.Fake<ISecurityTokenProvider>();
    private readonly IRenterRepository renterRepository = A.Fake<IRenterRepository>();
    private readonly IDocumentService documentService = A.Fake<IDocumentService>();
    private readonly IDriverLicenseService driverLicenseService = A.Fake<IDriverLicenseService>();

    private readonly RegisterCommandHandler sut;

    private readonly RegisterCommand command = new()
    {
        Email = "john@doe.com",
        Password = "JohnDoe123",
        GivenName = "John",
        FamilyName = "Doe",
        Birthdate = new DateOnly(2000, 09, 05),
        Document = "18.864.014/0001-19",
        DriverLicenseNumber = "92353762700",
        DriverLicenseCategory = "ab",
        DriverLicenseExpiry = new DateOnly(DateTime.Today.Year + 1, 01, 01)
    };

    private readonly string userId = Ulid.NewUlid().ToString();

    public RegisterCommandHandlerTests()
    {
        sut = new RegisterCommandHandler(
            userService,
            securityTokenProvider,
            renterRepository,
            documentService,
            driverLicenseService);

        A.CallTo(() => userService.CreateUserAsync(
                A<string>._,
                A<string>._,
                A<string[]>._,
                A<Dictionary<string, string>>._,
                A<CancellationToken>._))
            .Returns(userId);

        A.CallTo(() => documentService.IsUniqueAsync(A<Document>._, A<CancellationToken>._))
            .Returns(true);

        A.CallTo(() => driverLicenseService.IsUniqueAsync(A<DriverLicense>._, A<CancellationToken>._))
            .Returns(true);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnTokenResponse()
    {
        // Arrange
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
    public async Task Handle_WhenCommandIsValid_ShouldCreateRenterUser()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync(
                userId: userId,
                email: EmailAddress.Create(command.Email).Value,
                document: Document.Create(command.Document).Value,
                fullName: new FullName(command.GivenName, command.FamilyName),
                birthdate: Birthdate.Create(command.Birthdate).Value,
                driverLicense: DriverLicense.Create(
                    command.DriverLicenseNumber,
                    DriverLicenseCategory.FromName(command.DriverLicenseCategory, ignoreCase: true),
                    command.DriverLicenseExpiry).Value))
            .Value;

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => renterRepository.AddAsync(
                A<Renter>.That.Matches(
                    r => r.UserId == userId
                         && r.Document == renter.Document
                         && r.Email == renter.Email
                         && r.FullName == renter.FullName
                         && r.Birthdate == renter.Birthdate
                         && r.DriverLicense == renter.DriverLicense),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
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
    public async Task Handle_WhenCreateUserFails_ShouldNotCreateRenterUser()
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
        A.CallTo(() => renterRepository.AddAsync(A<Renter>._, A<CancellationToken>._))
            .MustNotHaveHappened();
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

    [Fact]
    public async Task Handle_WhenCreateRenterFails_ShouldReturnFailure()
    {
        // Arrange
        A.CallTo(() => driverLicenseService.IsUniqueAsync(A<DriverLicense>._, A<CancellationToken>._))
            .Returns(false); // Faz com que a criação do Renter falhe

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFailure();
    }

    [Fact]
    public async Task Handle_WhenCreateRenterFails_ShouldNotGenerateSecurityToken()
    {
        // Arrange
        A.CallTo(() => driverLicenseService.IsUniqueAsync(A<DriverLicense>._, A<CancellationToken>._))
            .Returns(false); // Faz com que a criação do Renter falhe

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => securityTokenProvider.GenerateSecurityTokenAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}