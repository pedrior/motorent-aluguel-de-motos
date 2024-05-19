using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.Errors;
using Motorent.Domain.Renters.Events;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.UnitTests.Renters;

[TestSubject(typeof(Renter))]
public sealed class RenterTests
{
    private readonly IDocumentService documentService = A.Fake<IDocumentService>();
    private readonly IDriverLicenseService driverLicenseService = A.Fake<IDriverLicenseService>();

    [Fact]
    public async Task CreateAsync_WhenCalled_ShouldCreateRenterWithPendingDriverLicenseValidationStatus()
    {
        // Arrange
        // Act
        var result = await Factories.Renter.CreateAsync();

        // Assert
        result.Should().BeSuccess();
        result.Value.DriverLicenseStatus.Should().Be(DriverLicenseStatus.PendingValidation);
    }

    [Fact]
    public async Task CreateAsync_WhenDocumentIsNotUnique_ShouldReturnDocumentIsNotUniquer()
    {
        // Arrange
        var document = Constants.Renter.Document;
        A.CallTo(() => documentService.IsUniqueAsync(document, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await Factories.Renter.CreateAsync(documentService: documentService);

        // Assert
        result.Should().BeFailure(RenterErrors.DocumentNotUnique(document));
    }

    [Fact]
    public async Task CreateAsync_WhenDriverLicenseIsNotUnique_ShouldReturnDriverLicenseIsNotUnique()
    {
        // Arrange
        var driverLicense = Constants.Renter.DriverLicense;
        A.CallTo(() => driverLicenseService.IsUniqueAsync(driverLicense, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await Factories.Renter.CreateAsync(driverLicenseService: driverLicenseService);

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotUnique(driverLicense));
    }

    [Fact]
    public async Task ChangePersonalInfo_WhenCalled_ShouldChangePersonalInfo()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        var newFullName = new FullName("Jane", "Doe");
        var newBirthdate = Birthdate.Create(new DateOnly(1990, 1, 1)).Value;

        // Act
        renter.ChangePersonalInfo(newFullName, newBirthdate);

        // Assert
        renter.FullName.Should().Be(newFullName);
        renter.Birthdate.Should().Be(newBirthdate);
    }

    [Fact]
    public async Task ChangeDriverLicenseAsync_WhenDriverLicenseStatusIsPendingValidation_ShouldChangeDriverLicense()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        var newDriverLicense = DriverLicense.Create(
            "42147507644",
            DriverLicenseCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => driverLicenseService.IsUniqueAsync(newDriverLicense, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await renter.ChangeDriverLicenseAsync(newDriverLicense, driverLicenseService);

        // Assert
        result.Should().BeSuccess();
        renter.DriverLicense.Should().Be(newDriverLicense);
    }

    [Fact]
    public async Task ChangeDriverLicenseAsync_WhenDriverLicenseStatusIsRejected_ShouldChangeDriverLicense()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);
        renter.RejectDriverLicense();

        var newDriverLicense = DriverLicense.Create(
            "42147507644",
            DriverLicenseCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => driverLicenseService.IsUniqueAsync(newDriverLicense, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await renter.ChangeDriverLicenseAsync(newDriverLicense, driverLicenseService);

        // Assert
        result.Should().BeSuccess();
        renter.DriverLicense.Should().Be(newDriverLicense);
    }

    [Fact]
    public async Task ChangeDriverLicenseAsync_WhenDriverLicenseStatusIsWaitingApproval_ShouldReturnDriverLicenseIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        var newDriverLicense = DriverLicense.Create(
            "42147507644",
            DriverLicenseCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => driverLicenseService.IsUniqueAsync(newDriverLicense, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await renter.ChangeDriverLicenseAsync(newDriverLicense, driverLicenseService);

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotPendingValidation);
    }

    [Fact]
    public async Task ChangeDriverLicenseAsync_WhenDriverLicenseStatusIsApproved_ShouldReturnDriverLicenseIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);
        renter.ApproveDriverLicense();

        var newDriverLicense = DriverLicense.Create(
            "42147507644",
            DriverLicenseCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => driverLicenseService.IsUniqueAsync(newDriverLicense, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await renter.ChangeDriverLicenseAsync(newDriverLicense, driverLicenseService);

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotPendingValidation);
    }

    [Fact]
    public async Task ChangeDriverLicenseAsync_WhenDriverLicenseIsNotUnique_ShouldReturnDriverLicenseNotUnique()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        var newDriverLicense = DriverLicense.Create(
            "42147507644",
            DriverLicenseCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => driverLicenseService.IsUniqueAsync(newDriverLicense, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await renter.ChangeDriverLicenseAsync(newDriverLicense, driverLicenseService);

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotUnique(newDriverLicense));
    }

    [Fact]
    public async Task SendDriverLicenseImage_WhenDriverLicenseStatusIsPendingValidation_ShouldSendDriverLicenseImage()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        // Assert
        result.Should().BeSuccess();
        renter.DriverLicenseStatus.Should().Be(DriverLicenseStatus.WaitingApproval);
        renter.DriverLicenseImageUrl.Should().Be(Constants.Renter.DriverLicenseImage);
    }

    [Fact]
    public async Task SendDriverLicenseImage_WhenDriverLicenseStatusIsRejected_ShouldSendDriverLicenseImage()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);
        renter.RejectDriverLicense();

        // Act
        var result = renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        // Assert
        result.Should().BeSuccess();
        renter.DriverLicenseStatus.Should().Be(DriverLicenseStatus.WaitingApproval);
        renter.DriverLicenseImageUrl.Should().Be(Constants.Renter.DriverLicenseImage);
    }

    [Fact]
    public async Task SendDriverLicenseImage_WhenCalled_ShouldRaiseDriverLicenseImageSent()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        // Assert
        result.Should().BeSuccess();
        renter.Events.Should().ContainSingle(e => e is DriverLicenseImageSent)
            .Which.As<DriverLicenseImageSent>().Should().BeEquivalentTo(new
            {
                RenterId = renter.Id, 
                renter.DriverLicenseImageUrl
            });
    }

    [Fact]
    public async Task SendDriverLicenseImage_WhenDriverLicenseStatusIsWaitingApproval_ShouldReturnDriverLicenseIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        // Act
        var result = renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotPendingValidation);
    }

    [Fact]
    public async Task SendDriverLicenseImage_WhenDriverLicenseStatusIsApproved_ShouldReturnDriverLicenseIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);
        renter.ApproveDriverLicense();

        // Act
        var result = renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotPendingValidation);
    }
    
    [Fact]
    public async Task ApproveDriverLicense_WhenDriverLicenseStatusIsWaitingApproval_ShouldSetApproveStatus()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        // Act
        var result = renter.ApproveDriverLicense();

        // Assert
        result.Should().BeSuccess();
        renter.DriverLicenseStatus.Should().Be(DriverLicenseStatus.Approved);
    }
    
    [Fact]
    public async Task ApproveDriverLicense_WhenDriverLicenseStatusIsPendingValidation_ShouldReturnDriverLicenseIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.ApproveDriverLicense();

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotWaitingApproval);
    }
    
    [Fact]
    public async Task ApproveDriverLicense_WhenDriverLicenseStatusIsRejected_ShouldReturnDriverLicenseIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);
        renter.RejectDriverLicense();

        // Act
        var result = renter.ApproveDriverLicense();

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotWaitingApproval);
    }
    
    [Fact]
    public async Task RejectDriverLicense_WhenDriverLicenseStatusIsWaitingApproval_ShouldSetRejectedStatus()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);

        // Act
        var result = renter.RejectDriverLicense();

        // Assert
        result.Should().BeSuccess();
        renter.DriverLicenseStatus.Should().Be(DriverLicenseStatus.Rejected);
        renter.DriverLicenseImageUrl.Should().BeNull();
    }

    [Fact]
    public async Task RejectDriverLicense_WhenDriverLicenseStatusIsPendingValidation_ShouldReturnDriverLicenseIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.RejectDriverLicense();

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotWaitingApproval);
    }
    
    [Fact]
    public async Task RejectDriverLicense_WhenDriverLicenseStatusIsApproved_ShouldReturnDriverLicenseIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendDriverLicenseImage(Constants.Renter.DriverLicenseImage);
        renter.ApproveDriverLicense();

        // Act
        var result = renter.RejectDriverLicense();

        // Assert
        result.Should().BeFailure(RenterErrors.DriverLicenseNotWaitingApproval);
    }
}