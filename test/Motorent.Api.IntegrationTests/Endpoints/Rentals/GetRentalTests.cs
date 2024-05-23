using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Rentals;

[TestSubject(typeof(RentalEndpoints))]
public sealed class GetRentalTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private static readonly RentalId RentalId = RentalId.New();
    private static readonly RentalPlan RentalPlan = RentalPlan.ThirtyDays;

    public override async Task InitializeAsync()
    {
        var userId = await CreateUserAsync(roles: [RenterUserRole], authenticate: true);
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;
        var motorcycle = (await Factories.Motorcycle.CreateAsync()).Value;
        var rental = Factories.Rental.Create(RentalId, renter.Id, motorcycle.Id, RentalPlan);

        await DataContext.Renters.AddAsync(renter);
        await DataContext.Motorcycles.AddAsync(motorcycle);
        await DataContext.Rentals.AddAsync(rental);

        await DataContext.SaveChangesAsync();

        await base.InitializeAsync();
    }

    [Fact]
    public async Task GetRental_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = Requests.Rental.GetRental(RentalId.Value);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRental_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        var request = Requests.Rental.GetRental(RentalId.Value);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRental_WhenUserIsNotRenter_ShouldReturnForbidden()
    {
        // Arrange
        await CreateUserAsync(
            email: "jane@admin.com",
            roles: [AdminUserRole],
            authenticate: true);

        var request = Requests.Rental.GetRental(RentalId.Value);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetRental_WhenRenterDoesNotOwnRental_ShouldReturnForbidden()
    {
        // Arrange
        DataContext.Renters.RemoveRange(DataContext.Renters);
        await DataContext.SaveChangesAsync();
        
        var userId = await CreateUserAsync(
            email: "john@renter.com",
            roles: [RenterUserRole],
            authenticate: true);

        var renter = (await Factories.Renter.CreateAsync(
            id: RenterId.New(),
            userId: userId)).Value;

        await DataContext.Renters.AddAsync(renter);
        await DataContext.SaveChangesAsync();

        var request = Requests.Rental.GetRental(RentalId.Value);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}