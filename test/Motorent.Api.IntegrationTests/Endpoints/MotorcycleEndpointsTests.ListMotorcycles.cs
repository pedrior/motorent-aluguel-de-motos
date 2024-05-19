using System.Text.Json;
using Motorent.Contracts.Common.Responses;
using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed partial class MotorcycleEndpointsTests
{
    [Fact]
    public async Task ListMotorcycles_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        await CreateMotorcyclesAsync(5);

        var request = Requests.Motorcycle.ListMotorcycles();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var motorcycles = JsonSerializer.Deserialize<PageResponse<MotorcycleSummaryResponse>>(
            content, SerializerOptions);

        motorcycles.Should().BeEquivalentTo(new
        {
            Requests.Motorcycle.ListMotorcyclesRequest.Page,
            Requests.Motorcycle.ListMotorcyclesRequest.Limit,
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
        await CreateMotorcyclesAsync(5);

        var request = Requests.Motorcycle.ListMotorcycles(Requests.Motorcycle.ListMotorcyclesRequest with
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