using Motorent.Contracts.Renters.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Renter
    {
        public static readonly UpdatePersonalInfoRequest UpdatePersonalInfoRequest = new()
        {
            GivenName = "Jane",
            FamilyName = "Doe",
            Birthdate = new DateOnly(1990, 1, 1)
        };
        
        public static readonly UpdateDriverLicenseRequest UpdateDriverLicenseRequest = new()
        {
            Number = "19452106448",
            Category = "B",
            Expiry = new DateOnly(DateTime.Today.Year + 2, 10, 25)
        };

        public static HttpRequestMessage GetProfile() => Get("v1/renters");

        public static HttpRequestMessage UpdatePersonalInfo(UpdatePersonalInfoRequest? request = null) =>
            Put("v1/renters", request ?? UpdatePersonalInfoRequest);
        
        public static HttpRequestMessage UpdateDriverLicense(UpdateDriverLicenseRequest? request = null) =>
            Put("v1/renters/driver-license", request ?? UpdateDriverLicenseRequest);
    }
}