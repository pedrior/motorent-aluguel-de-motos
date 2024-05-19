using Motorent.Contracts.Motorcycles.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Motorcycle
    {
        public static readonly RegisterMotorcycleRequest RegisterMotorcycleRequest = new()
        {
            Model = "Titan 160cc ABS",
            Year = DateTime.Today.Year - 2,
            LicensePlate = "PIA2A91"
        };

        public static readonly ListMotorcyclesRequest ListMotorcyclesRequest = new()
        {
            Page = 1,
            Limit = 10,
            Sort = "asc",
            Order = null,
            Search = null
        };

        public static readonly UpdateLicensePlateRequest UpdateLicensePlateRequest = new()
        {
            LicensePlate = "KIL2H17"
        };

        public static HttpRequestMessage RegisterMotorcycle(RegisterMotorcycleRequest? request = null) =>
            Post("v1/motorcycles", request ?? RegisterMotorcycleRequest);

        public static HttpRequestMessage GetMotorcycle(string idOrLicensePlate) =>
            Get($"v1/motorcycles/{idOrLicensePlate}");

        public static HttpRequestMessage ListMotorcycles(ListMotorcyclesRequest? request = null)
        {
            request ??= ListMotorcyclesRequest;
            return Get($"v1/motorcycles" +
                       $"?page={request.Page}" +
                       $"&limit={request.Limit}" +
                       $"&sort={request.Sort}" +
                       $"&order={request.Order}" +
                       $"&search={request.Search}");
        }
        
        public static HttpRequestMessage UpdateLicensePlate(string idOrLicensePlate,
            UpdateLicensePlateRequest? request = null)
        {
            return Put($"v1/motorcycles/{idOrLicensePlate}/license-plate", request ?? UpdateLicensePlateRequest);
        }

        public static HttpRequestMessage DeleteMotorcycle(Ulid id) => Delete($"v1/motorcycles/{id}");
    }
}