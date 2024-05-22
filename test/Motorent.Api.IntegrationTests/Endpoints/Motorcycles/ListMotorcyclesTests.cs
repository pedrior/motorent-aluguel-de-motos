using System.Text.Json;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Requests;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Motorcycles;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed class ListMotorcyclesTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private static readonly ListMotorcyclesRequest ListMotorcyclesRequest = new()
    {
        Page = 1,
        Limit = 10,
        Sort = "asc",
        Order = null,
        Search = null
    };

    public override async Task InitializeAsync()
    {
        await CreateMotorcyclesAsync();

        await base.InitializeAsync();
    }

    private async Task CreateMotorcyclesAsync()
    {
        var motorcycles = new List<Motorcycle>();
        
        for (var i = 0; i < 5; i++)
        {
            var motorcycle = await Factories.Motorcycle.CreateAsync(
                id: MotorcycleId.New(),
                licensePlate: LicensePlate.Create($"KIL{i % 9}H{i + 1 % 9}{i + 2 % 9}").Value);
            
            motorcycles.Add(motorcycle.Value);
        }

        await DataContext.Motorcycles.AddRangeAsync(motorcycles);
        await DataContext.SaveChangesAsync();
    }

    [Fact]
    public async Task ListMotorcycles_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = Requests.Motorcycle.ListMotorcycles(ListMotorcyclesRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var motorcycles = JsonSerializer.Deserialize<PageResponse<MotorcycleSummaryResponse>>(
            content, SerializerOptions);

        motorcycles.Should().BeEquivalentTo(new
        {
            ListMotorcyclesRequest.Page,
            ListMotorcyclesRequest.Limit,
            TotalItems = 5,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        });
    }

    [Theory]
    [InlineData(0, 5, "asc", "model")] // Page is 0
    [InlineData(1, 0, "desc", "model")] // Limit is 0
    [InlineData(1, 5, "foo", "model")] // Order is invalid
    [InlineData(1, 5, "asc", "foo")] // Sort is invalid
    public async Task ListMotorcycles_WhenRequestIsInvalid_ShouldReturnBadRequest(
        int page,
        int limit,
        string? order,
        string? sort)
    {
        // Arrange
        var request = Requests.Motorcycle.ListMotorcycles(ListMotorcyclesRequest with
        {
            Page = page,
            Limit = limit,
            Order = order,
            Sort = sort
        });

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}