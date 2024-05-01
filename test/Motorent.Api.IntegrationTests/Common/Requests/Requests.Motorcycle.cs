using Motorent.Contracts.Motorcycles.Requests;

namespace Motorent.Api.IntegrationTests.Common.Requests;

internal static partial class Requests
{
    public static class Motorcycle
    {
        public static readonly RegisterMotorcycleRequest RegisterMotorcycleRequest = new()
        {
            Model = "Titan 160cc ABS",
            Brand = "honda",
            Year = DateTime.Today.Year - 2,
            DailyPrice = 38.99m,
            LicensePlate = "PIA-2A91"
        };

        public static readonly ChangeLicensePlateRequest ChangeLicensePlateRequest = new()
        {
            LicensePlate = "KIL-2H17"
        };

        public static HttpRequestMessage RegisterMotorcycle(RegisterMotorcycleRequest? request = null) =>
            Post("v1/motorcycles", request ?? RegisterMotorcycleRequest);

        public static HttpRequestMessage GetMotorcycle(string idOrLicensePlate) =>
            Get($"v1/motorcycles/{idOrLicensePlate}");

        public static HttpRequestMessage ChangeLicensePlate(string idOrLicensePlate,
            ChangeLicensePlateRequest? request = null)
        {
            return Post($"v1/motorcycles/{idOrLicensePlate}/change-license-plate",
                request ?? ChangeLicensePlateRequest);
        }
    }
}