using Motorent.Contracts.Renters.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Renter
    {
        public static HttpRequestMessage GetProfile() => Get("v1/renters");

        public static HttpRequestMessage UpdatePersonalInfo(UpdatePersonalInfoRequest request) =>
            Put("v1/renters", request);
        
        public static HttpRequestMessage UpdateDriverLicense(UpdateDriverLicenseRequest request) =>
            Put("v1/renters/driver-license", request);
    }
}