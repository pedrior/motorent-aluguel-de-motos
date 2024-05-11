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
        
        public static readonly UpdateCNHRequest UpdateCNHRequest = new()
        {
            Number = "19452106448",
            Category = "B",
            ExpDate = new DateOnly(DateTime.Today.Year + 2, 10, 25)
        };

        public static HttpRequestMessage GetRenterProfile() => Get("v1/renters");

        public static HttpRequestMessage UpdatePersonalInfo(UpdatePersonalInfoRequest? request = null) =>
            Put("v1/renters/personal-info", request ?? UpdatePersonalInfoRequest);
        
        public static HttpRequestMessage UpdateCNH(UpdateCNHRequest? request = null) =>
            Put("v1/renters/cnh", request ?? UpdateCNHRequest);
    }
}