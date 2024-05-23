using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Rentals;

[TestSubject(typeof(RentalEndpoints))]
public sealed class RentTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private static readonly string Plan = RentalPlan.FifteenDays.Name;
    private static readonly string MotorcycleId = Ulid.NewUlid().ToString();

    public override async Task InitializeAsync()
    {
        var userId = await CreateUserAsync(roles: [RenterUserRole], authenticate: true);

        await CreateRenterAsync(userId);

        await CreateMotorcycleAsync();

        await base.InitializeAsync();
    }

    private async Task CreateRenterAsync(string userId)
    {
        var renter = (await Factories.Renter.CreateAsync(userId: userId))
            .Value;

        await DataContext.Renters.AddAsync(renter);
        await DataContext.SaveChangesAsync();
    }

    private async Task CreateMotorcycleAsync()
    {
        var motorcycle = (await Factories.Motorcycle.CreateAsync(
                id: new MotorcycleId(Ulid.Parse(MotorcycleId))))
            .Value;

        await DataContext.Motorcycles.AddAsync(motorcycle);
        await DataContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Rent_WhenRequestIsValid_ShouldCreateRental()
    {
        // Arrange
        var request = Requests.Rental.Rent(Plan, MotorcycleId);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.DeserializeContentAsync<RentalResponse>();

        var rental = DataContext.Rentals.SingleOrDefaultAsync(
            r => r.Id == new RentalId(Ulid.Parse(content.Id)));
        
        rental.Should().NotBeNull();
    }

    [Fact]
    public async Task Rent_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        var request = Requests.Rental.Rent(Plan, MotorcycleId);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Rent_WhenUserIsNotAuthorized_ShouldReturnForbidden()
    {
        // Arrange
        await CreateUserAsync(
            email: "john@admin.com",
            roles: [AdminUserRole],
            authenticate: true);

        var request = Requests.Rental.Rent(Plan, MotorcycleId);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Rent_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Rental.Rent(Plan, Ulid.NewUlid().ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}