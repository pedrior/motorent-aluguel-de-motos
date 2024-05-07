using Motorent.Contracts.Renters.Requests;

namespace Motorent.Api.IntegrationTests.Common.Requests;

internal static partial class Requests
{
    public static class Renter
    {
        public static readonly UpdatePersonalInformationRequest UpdatePersonalInformationRequest = new()
        {
            GivenName = "Jane",
            FamilyName = "Doe",
            Birthdate = new DateOnly(1990, 1, 1)
        };

        public static HttpRequestMessage GetRenterProfile() => Get("v1/renters/profile");

        public static HttpRequestMessage UpdatePersonalInformation(UpdatePersonalInformationRequest? request = null) =>
            Put("v1/renters/personal-information", request ?? UpdatePersonalInformationRequest);
    }
}