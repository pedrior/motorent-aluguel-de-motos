using Motorent.Contracts.Rentals.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Rental
    {
        public static HttpRequestMessage Rent(string plan, Ulid motorcycleId) =>
            Post("v1/rentals", new RentRequest
            {
                Plan = plan,
                MotorcycleId = motorcycleId
            });
    }
}