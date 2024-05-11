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
    private readonly ICNPJService cnpjService = A.Fake<ICNPJService>();
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
    public async Task CreateAsync_WhenCNPJIsNotUnique_ShouldReturnCnpjIsNotUniquer()
    {
        // Arrange
        var cnpj = Constants.Renter.CNPJ;
        A.CallTo(() => cnpjService.IsUniqueAsync(cnpj, A<CancellationToken>._))
            .Returns(false);

        // Act
        var result = await Factories.Renter.CreateAsync(cnpjService: cnpjService);

        // Assert
        result.Should().BeFailure(RenterErrors.CNPJNotUnique(cnpj));
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
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);
        renter.SetCNHRejectedStatus();

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
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

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
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);
        renter.SetCNHApprovedStatus();

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
    public async Task SendCNHImagesForValidation_WhenCNHStatusIsPendingValidation_ShouldSendCNHImagesForValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Assert
        result.Should().BeSuccess();
        renter.CNHStatus.Should().Be(CNHStatus.WaitingApproval);
        renter.CNHValidationImages.Should().Be(Constants.Renter.CNHValidationImages);
    }

    [Fact]
    public async Task SendCNHImagesForValidation_WhenCNHStatusIsRejected_ShouldSendCNHImagesForValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);
        renter.SetCNHRejectedStatus();

        // Act
        var result = renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Assert
        result.Should().BeSuccess();
        renter.CNHStatus.Should().Be(CNHStatus.WaitingApproval);
        renter.CNHValidationImages.Should().Be(Constants.Renter.CNHValidationImages);
    }

    [Fact]
    public async Task SendCNHImagesForValidation_WhenCalled_ShouldRaiseCNHImagesSentForValidationEvent()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Assert
        result.Should().BeSuccess();
        renter.Events.Should().ContainSingle(e => e is CNHImagesSentForValidationEvent)
            .Which.As<CNHImagesSentForValidationEvent>().Should().BeEquivalentTo(new
            {
                RenterId = renter.Id,
                renter.CNHValidationImages
            });
    }

    [Fact]
    public async Task SendCNHImagesForValidation_WhenCNHStatusIsWaitingApproval_ShouldReturnCNHIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Act
        var result = renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotPendingValidation);
    }

    [Fact]
    public async Task SendCNHImagesForValidation_WhenCNHStatusIsApproved_ShouldReturnCNHIsNotPendingValidation()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);
        renter.SetCNHApprovedStatus();

        // Act
        var result = renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotPendingValidation);
    }
    
    [Fact]
    public async Task SetCNHApprovedStatus_WhenCNHStatusIsWaitingApproval_ShouldSetCNHApprovedStatus()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Act
        var result = renter.SetCNHApprovedStatus();

        // Assert
        result.Should().BeSuccess();
        renter.CNHStatus.Should().Be(CNHStatus.Approved);
    }
    
    [Fact]
    public async Task SetCNHApprovedStatus_WhenCalled_ShouldRaiseCNHStatusChangedToApprovedEvent()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Act
        var result = renter.SetCNHApprovedStatus();

        // Assert
        result.Should().BeSuccess();
        renter.Events.Should().ContainSingle(e => e is CNHStatusChangedToApprovedEvent)
            .Which.As<CNHStatusChangedToApprovedEvent>().Should().BeEquivalentTo(new
            {
                RenterId = renter.Id
            });
    }
    
    [Fact]
    public async Task SetCNHApprovedStatus_WhenCNHStatusIsPendingValidation_ShouldReturnCNHIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.SetCNHApprovedStatus();

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotWaitingApproval);
    }
    
    [Fact]
    public async Task SetCNHApprovedStatus_WhenCNHStatusIsRejected_ShouldReturnCNHIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);
        renter.SetCNHRejectedStatus();

        // Act
        var result = renter.SetCNHApprovedStatus();

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotWaitingApproval);
    }
    
    [Fact]
    public async Task SetCNHRejectedStatus_WhenCNHStatusIsWaitingApproval_ShouldSetCNHRejectedStatus()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Act
        var result = renter.SetCNHRejectedStatus();

        // Assert
        result.Should().BeSuccess();
        renter.CNHStatus.Should().Be(CNHStatus.Rejected);
        renter.CNHValidationImages.Should().BeNull();
    }
    
    [Fact]
    public async Task SetCNHRejectedStatus_WhenCalled_ShouldRaiseCNHStatusChangedToRejectedEvent()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);

        // Act
        var result = renter.SetCNHRejectedStatus();

        // Assert
        result.Should().BeSuccess();
        renter.Events.Should().ContainSingle(e => e is CNHStatusChangedToRejectedEvent)
            .Which.As<CNHStatusChangedToRejectedEvent>().Should().BeEquivalentTo(new
            {
                RenterId = renter.Id
            });
    }
    
    [Fact]
    public async Task SetCNHRejectedStatus_WhenCNHStatusIsPendingValidation_ShouldReturnCNHIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;

        // Act
        var result = renter.SetCNHRejectedStatus();

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotWaitingApproval);
    }
    
    [Fact]
    public async Task SetCNHRejectedStatus_WhenCNHStatusIsApproved_ShouldReturnCNHIsNotWaitingApproval()
    {
        // Arrange
        var renter = (await Factories.Renter.CreateAsync()).Value;
        renter.SendCNHImagesForValidation(Constants.Renter.CNHValidationImages);
        renter.SetCNHApprovedStatus();

        // Act
        var result = renter.SetCNHRejectedStatus();

        // Assert
        result.Should().BeFailure(RenterErrors.CNHIsNotWaitingApproval);
    }
}