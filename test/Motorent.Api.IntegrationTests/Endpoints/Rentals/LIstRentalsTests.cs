using Motorent.Api.IntegrationTests.TestUtils;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Rentals;

[TestSubject(typeof(RentalEndpoints))]
public sealed class ListRentalTests(IntegrationTestWebApplicationFactory api) : AbstractIntegrationTest(api)
{
    public override async Task InitializeAsync()
    {
        var userId = await CreateUserAsync(roles: [RenterUserRole], authenticate: true);
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        var rental = Factories.Rental.Create(renterId: renter.Id, motorcycleId: motorcycle.Id);

        await DataContext.Renters.AddAsync(renter);
        await DataContext.Motorcycles.AddAsync(motorcycle);
        await DataContext.Rentals.AddAsync(rental);

        await DataContext.SaveChangesAsync();

        await base.InitializeAsync();
    }

    [Fact]
    public async Task ListRentals_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = Requests.Rental.ListRentals();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListRentals_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        var request = Requests.Rental.ListRentals();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListRentals_WhenUserIsNotRenter_ShouldReturnForbidden()
    {
        // Arrange
        await CreateUserAsync(
            email: "jane@admin.com",
            roles: [AdminUserRole],
            authenticate: true);

        var request = Requests.Rental.ListRentals();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}