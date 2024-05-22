using Motorent.Contracts.Motorcycles.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Motorcycle
    {
        public static HttpRequestMessage RegisterMotorcycle(RegisterMotorcycleRequest request) => 
            Post("v1/motorcycles", request);

        public static HttpRequestMessage GetMotorcycle(string idOrLicensePlate) =>
            Get($"v1/motorcycles/{idOrLicensePlate}");

        public static HttpRequestMessage ListMotorcycles(ListMotorcyclesRequest request)
        {
            return Get($"v1/motorcycles" +
                       $"?page={request.Page}" +
                       $"&limit={request.Limit}" +
                       $"&sort={request.Sort}" +
                       $"&order={request.Order}" +
                       $"&search={request.Search}");
        }
        
        public static HttpRequestMessage UpdateLicensePlate(
            string idOrLicensePlate, UpdateLicensePlateRequest request)
        {
            return Put($"v1/motorcycles/{idOrLicensePlate}/license-plate", request);
        }

        public static HttpRequestMessage DeleteMotorcycle(Ulid id) => Delete($"v1/motorcycles/{id}");
    }
}