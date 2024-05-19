using Motorent.Domain.Renters;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(RenterEndpoints))]
public sealed partial class RenterEndpointsTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private async Task<Renter> CreateRenterAsync(string userId, string? cnhNumber = null)
    {
        var renter = (await Factories.Renter.CreateAsync(
            userId: userId,
            cnh: CNH.Create(
                number: cnhNumber ?? Constants.Renter.CNH.Number,
                category: Constants.Renter.CNH.Category,
                expirationDate: Constants.Renter.CNH.ExpirationDate).Value)).Value;

        await DataContext.Renters.AddAsync(renter);
        await DataContext.SaveChangesAsync();

        return renter;
    }
}