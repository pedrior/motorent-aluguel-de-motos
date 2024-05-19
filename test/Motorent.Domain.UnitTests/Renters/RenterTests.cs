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
    private readonly ICNHService cnhService = A.Fake<ICNHService>();

    [Fact]
    public async Task CreateAsync_WhenCalled_ShouldCreateRenterWithPendingCNHValidationStatus()
    {
        // Arrange
        // Act
        var result = await Factories.Renter.CreateAsync();

        // Assert
        result.Should().BeSuccess();
        result.Value.CNHStatus.Should().Be(CNHStatus.PendingValidation);
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
    public async Task CreateAsync_WhenCNHIsNotUnique_ShouldReturnCNHIsNotUnique()
    {
        // Arrange
        var cnh = Constants.Renter.CNH;
        A.CallTo(() => cnhService.IsUniqueAsync(cnh, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await Factories.Renter.CreateAsync(cnhService: cnhService);

        // Assert
        result.Should().BeFailure(RenterErrors.CNHNotUnique(cnh));
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
    public async Task ChangeCNHAsync_WhenCNHStatusIsPendingValidation_ShouldChangeCNH()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        var newCNH = CNH.Create(
            "42147507644",
            CNHCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => cnhService.IsUniqueAsync(newCNH, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await renter.ChangeCNHAsync(newCNH, cnhService);

        // Assert
        result.Should().BeSuccess();
        renter.CNH.Should().Be(newCNH);
    }

    [Fact]
    public async Task ChangeCNHAsync_WhenCNHStatusIsRejected_ShouldChangeCNH()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);
        renter.RejectCNH();

        var newCNH = CNH.Create(
            "42147507644",
            CNHCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => cnhService.IsUniqueAsync(newCNH, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await renter.ChangeCNHAsync(newCNH, cnhService);

        // Assert
        result.Should().BeSuccess();
        renter.CNH.Should().Be(newCNH);
    }

    [Fact]
    public async Task ChangeCNHAsync_WhenCNHStatusIsWaitingApproval_ShouldReturnCNHIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);

        var newCNH = CNH.Create(
            "42147507644",
            CNHCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => cnhService.IsUniqueAsync(newCNH, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await renter.ChangeCNHAsync(newCNH, cnhService);

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotPendingValidation);
    }

    [Fact]
    public async Task ChangeCNHAsync_WhenCNHStatusIsApproved_ShouldReturnCNHIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);
        renter.ApproveCNH();

        var newCNH = CNH.Create(
            "42147507644",
            CNHCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => cnhService.IsUniqueAsync(newCNH, A<CancellationToken>._))
            .Returns(true);

        // Act
        var result = await renter.ChangeCNHAsync(newCNH, cnhService);

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotPendingValidation);
    }

    [Fact]
    public async Task ChangeCNHAsync_WhenCNHIsNotUnique_ShouldReturnCNHNotUnique()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        var newCNH = CNH.Create(
            "42147507644",
            CNHCategory.A,
            new DateOnly(DateTime.Today.Year + 2, 01, 01)).Value;

        A.CallTo(() => cnhService.IsUniqueAsync(newCNH, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await renter.ChangeCNHAsync(newCNH, cnhService);

        // Assert
        result.Should().BeFailure(RenterErrors.CNHNotUnique(newCNH));
    }

    [Fact]
    public async Task SendCNHImage_WhenCNHStatusIsPendingValidation_ShouldSendCNHImage()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.SendCNHImage(Constants.Renter.CNHImage);

        // Assert
        result.Should().BeSuccess();
        renter.CNHStatus.Should().Be(CNHStatus.WaitingApproval);
        renter.CNHImageUrl.Should().Be(Constants.Renter.CNHImage);
    }

    [Fact]
    public async Task SendCNHImage_WhenCNHStatusIsRejected_ShouldSendCNHImage()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);
        renter.RejectCNH();

        // Act
        var result = renter.SendCNHImage(Constants.Renter.CNHImage);

        // Assert
        result.Should().BeSuccess();
        renter.CNHStatus.Should().Be(CNHStatus.WaitingApproval);
        renter.CNHImageUrl.Should().Be(Constants.Renter.CNHImage);
    }

    [Fact]
    public async Task SendCNHImage_WhenCalled_ShouldRaiseCNHImageSent()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.SendCNHImage(Constants.Renter.CNHImage);

        // Assert
        result.Should().BeSuccess();
        renter.Events.Should().ContainSingle(e => e is CNHImageSent)
            .Which.As<CNHImageSent>().Should().BeEquivalentTo(new
            {
                RenterId = renter.Id,
                renter.CNHImageUrl
            });
    }

    [Fact]
    public async Task SendCNHImage_WhenCNHStatusIsWaitingApproval_ShouldReturnCNHIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);

        // Act
        var result = renter.SendCNHImage(Constants.Renter.CNHImage);

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotPendingValidation);
    }

    [Fact]
    public async Task SendCNHImage_WhenCNHStatusIsApproved_ShouldReturnCNHIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);
        renter.ApproveCNH();

        // Act
        var result = renter.SendCNHImage(Constants.Renter.CNHImage);

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotPendingValidation);
    }
    
    [Fact]
    public async Task ApproveCNH_WhenCNHStatusIsWaitingApproval_ShouldSetApproveStatus()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);

        // Act
        var result = renter.ApproveCNH();

        // Assert
        result.Should().BeSuccess();
        renter.CNHStatus.Should().Be(CNHStatus.Approved);
    }
    
    [Fact]
    public async Task ApproveCNH_WhenCNHStatusIsPendingValidation_ShouldReturnCNHIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.ApproveCNH();

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotWaitingApproval);
    }
    
    [Fact]
    public async Task ApproveCNH_WhenCNHStatusIsRejected_ShouldReturnCNHIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);
        renter.RejectCNH();

        // Act
        var result = renter.ApproveCNH();

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotWaitingApproval);
    }
    
    [Fact]
    public async Task RejectCNH_WhenCNHStatusIsWaitingApproval_ShouldSetRejectedStatus()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);

        // Act
        var result = renter.RejectCNH();

        // Assert
        result.Should().BeSuccess();
        renter.CNHStatus.Should().Be(CNHStatus.Rejected);
        renter.CNHImageUrl.Should().BeNull();
    }

    [Fact]
    public async Task RejectCNH_WhenCNHStatusIsPendingValidation_ShouldReturnCNHIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.RejectCNH();

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotWaitingApproval);
    }
    
    [Fact]
    public async Task RejectCNH_WhenCNHStatusIsApproved_ShouldReturnCNHIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImage(Constants.Renter.CNHImage);
        renter.ApproveCNH();

        // Act
        var result = renter.RejectCNH();

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotWaitingApproval);
    }
}