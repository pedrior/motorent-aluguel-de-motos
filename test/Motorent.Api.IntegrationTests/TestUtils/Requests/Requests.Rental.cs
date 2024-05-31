using Motorent.Contracts.Rentals.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Rental
    {
        public static HttpRequestMessage Rent(string plan, string motorcycleId)
        {
            return Post("v1/rentals", new RentRequest
            {
                Plan = plan,
                MotorcycleId = motorcycleId
            });
        }
        
        public static HttpRequestMessage GetRental(Ulid rentalId) => Get($"v1/rentals/{rentalId}");
        
        public static HttpRequestMessage ListRentals(int page = 1, int limit = 10) =>
            Get($"v1/rentals?page={page}&limit={limit}");

        public static HttpRequestMessage UpdateReturnDate(Ulid rentalId, DateOnly returnDate)
        {
            return Put($"v1/rentals/{rentalId}/return-date", new UpdateReturnDateRequest
            {
                ReturnDate = returnDate
            });
        }
    }
}