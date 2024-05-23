using Motorent.Contracts.Rentals.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Rental
    {
        public static HttpRequestMessage Rent(string plan, Ulid motorcycleId)
        {
            return Post("v1/rentals", new RentRequest
            {
                Plan = plan,
                MotorcycleId = motorcycleId
            });
        }

        public static HttpRequestMessage UpdateReturnDate(Ulid rentalId, DateOnly returnDate)
        {
            return Put($"v1/rentals/{rentalId}/return-date", new UpdateReturnDateRequest
            {
                ReturnDate = returnDate
            });
        }
    }
}