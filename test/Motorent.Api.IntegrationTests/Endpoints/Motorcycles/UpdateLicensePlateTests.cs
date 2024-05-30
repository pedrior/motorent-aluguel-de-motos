using Motorent.Api.IntegrationTests.TestUtils;
using Motorent.Contracts.Motorcycles.Requests;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Motorcycles;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed class UpdateLicensePlateTests(IntegrationTestWebApplicationFactory api) : AbstractIntegrationTest(api)
{
    private static readonly MotorcycleId MotorcycleId = MotorcycleId.New();
    private static readonly LicensePlate LicensePlate = LicensePlate.Create("KIL2H17").Value;

    private UpdateLicensePlateRequest updateLicensePlateRequest = null!;

    public override async Task InitializeAsync()
    {
        updateLicensePlateRequest = new UpdateLicensePlateRequest
        {
            LicensePlate = "KIL2H17"
        };

        await CreateMotorcycleAsync();

        await CreateUserAsync(roles: [AdminUserRole], authenticate: true);

        await base.InitializeAsync();
    }

    private async Task CreateMotorcycleAsync()
    {
        var motorcycle = await Factories.Motorcycle.CreateAsync(
            id: MotorcycleId,
            licensePlate: LicensePlate);

        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateLicensePlate_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var request = Requests.Motorcycle.UpdateLicensePlate(
            MotorcycleId.ToString(), updateLicensePlateRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateLicensePlate_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        var request = Requests.Motorcycle.UpdateLicensePlate(
            MotorcycleId.ToString(), updateLicensePlateRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangeLicensePlace_WhenUserIsNotAuthorized_ShouldReturnForbidden()
    {
        // Arrange
        await CreateUserAsync(
            email: "john@renter.com",
            roles: [RenterUserRole],
            authenticate: true);

        var request = Requests.Motorcycle.UpdateLicensePlate(
            MotorcycleId.ToString(), updateLicensePlateRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateLicensePlate_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Motorcycle.UpdateLicensePlate(
            Ulid.NewUlid().ToString(), updateLicensePlateRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}