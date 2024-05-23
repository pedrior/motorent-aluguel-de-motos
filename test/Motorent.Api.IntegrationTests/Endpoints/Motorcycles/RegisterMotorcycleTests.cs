using Motorent.Contracts.Motorcycles.Requests;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Motorcycles;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed class RegisterMotorcycleTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private RegisterMotorcycleRequest registerMotorcycleRequest = null!;

    public override async Task InitializeAsync()
    {
        registerMotorcycleRequest = new RegisterMotorcycleRequest
        {
            Model = "Titan 160cc ABS",
            LicensePlate = "PIA2A91",
            Year = DateTime.Today.Year
        };
        
        await CreateUserAsync(roles: [AdminUserRole], authenticate: true);
        
        await base.InitializeAsync();
    }

    [Fact]
    public async Task RegisterMotorcycle_WhenRequestIsValid_ShouldCreateMotorcycle()
    {
        // Arrange
        var request = Requests.Motorcycle.RegisterMotorcycle(registerMotorcycleRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.DeserializeContentAsync<MotorcycleResponse>();
        var motorcycle = await DataContext.Motorcycles.SingleAsync(
            m => m.Id == new MotorcycleId(Ulid.Parse(content.Id)));

        motorcycle.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterMotorcycle_WhenLicensePlateIsDuplicated_ShouldReturnConflict()
    {
        // Arrange
        var motorcycle = await Factories.Motorcycle.CreateAsync(
            licensePlate: LicensePlate.Create(registerMotorcycleRequest.LicensePlate).Value);

        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();

        var request = Requests.Motorcycle.RegisterMotorcycle(registerMotorcycleRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}