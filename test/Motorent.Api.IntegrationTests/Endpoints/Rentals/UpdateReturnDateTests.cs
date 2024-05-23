using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Rentals;

[TestSubject(typeof(RentalEndpoints))]
public sealed class UpdateReturnDateTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private static readonly RentalId RentalId = RentalId.New();
    private static readonly RentalPlan RentalPlan = RentalPlan.ThirtyDays;
    
    private readonly HttpRequestMessage request = Requests.Rental.UpdateReturnDate(
        RentalId.Value,
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(RentalPlan.Days + 10)));

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
    public async Task UpdateReturnDate_WhenRequestIsValid_ShouldUpdateRental()
    {
        // Arrange
        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task UpdateReturnDate_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();
        
        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateReturnDate_WhenUserIsNotAuthorized_ShouldReturnForbidden()
    {
        // Arrange
        await CreateUserAsync(
            email: "john@admin.com",
            roles: [AdminUserRole],
            authenticate: true);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task UpdateReturnDate_WhenRentalDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var newRequest = Requests.Rental.UpdateReturnDate(
            RentalId.New().Value,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(RentalPlan.Days + 10)));

        // Act
        var response = await Client.SendAsync(newRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}