using System.Text.Json;
using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(RentalEndpoints))]
public sealed partial class RentalEndpointsTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private static readonly string Plan = RentalPlan.FifteenDays.Name;
    private static readonly Ulid MotorcycleId = Ulid.NewUlid();

    public override async Task InitializeAsync()
    {
        var userId = await CreateUserAsync(roles: [UserRoles.Renter]);
        await AuthenticateUserAsync(userId);

        var renter = (await Factories.Renter.CreateAsync(userId: userId))
            .Value;

        var motorcycle = (await Factories.Motorcycle.CreateAsync(
                id: new MotorcycleId(MotorcycleId)))
            .Value;

        await DataContext.Renters.AddAsync(renter);
        await DataContext.Motorcycles.AddAsync(motorcycle);
        
        await DataContext.SaveChangesAsync();

        await base.InitializeAsync();
    }

    [Fact]
    public async Task Rent_WhenRequestIsValid_ShouldReturnRentalResponse()
    {
        // Arrange
        var request = Requests.Rental.Rent(Plan, MotorcycleId);

        // Act
        var message = await Client.SendAsync(request);

        // Assert
        var content = await message.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<RentalResponse>(content, SerializerOptions);

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task Rent_WhenRequestIsValid_ShouldCreateRental()
    {
        // Arrange
        var request = Requests.Rental.Rent(Plan, MotorcycleId);

        // Act
        var message = await Client.SendAsync(request);

        // Assert
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await message.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<RentalResponse>(content, SerializerOptions);

        var rental = DataContext.Rentals.SingleOrDefaultAsync(r => r.Id == new RentalId(response!.Id));
        rental.Should().NotBeNull();
    }

    [Fact]
    public async Task Rent_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        var request = Requests.Rental.Rent(Plan, MotorcycleId);

        // Act
        var message = await Client.SendAsync(request);

        // Assert
        message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Rent_WhenUserIsNotAuthorized_ShouldReturnForbidden()
    {
        // Arrange
        var userId = await CreateUserAsync(
            email: "john@admin.com",
            roles: [UserRoles.Admin]);

        await AuthenticateUserAsync(userId);

        var request = Requests.Rental.Rent(Plan, MotorcycleId);

        // Act
        var message = await Client.SendAsync(request);

        // Assert
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Rent_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Rental.Rent(Plan, Ulid.NewUlid());

        // Act
        var message = await Client.SendAsync(request);

        // Assert
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}