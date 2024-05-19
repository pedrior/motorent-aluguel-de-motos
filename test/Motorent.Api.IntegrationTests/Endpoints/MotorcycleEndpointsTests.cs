using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed partial class MotorcycleEndpointsTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private async Task<MotorcycleId> CreateMotorcycleAsync(LicensePlate? licensePlate = null)
    {
        var motorcycle = await Factories.Motorcycle.CreateAsync(licensePlate: licensePlate);
        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();

        return motorcycle.Value.Id;
    }
    
    private async Task CreateMotorcyclesAsync(int count)
    {
        var motorcycles = new List<Motorcycle>();
        for (var i = 0; i < count; i++)
        {
            var id = MotorcycleId.New();
            var licensePlate = LicensePlate.Create($"PKP{i % 9}A{i + 1 % 9}{i + 2 % 9}").Value;
            motorcycles.Add((await Factories.Motorcycle.CreateAsync(id: id, licensePlate: licensePlate)).Value);
        }

        await DataContext.Motorcycles.AddRangeAsync(motorcycles);
        await DataContext.SaveChangesAsync();
    }
}