namespace Motorent.Api.IntegrationTests.Common.Requests;

internal static partial class Requests
{
    public static class Renter
    {
        public static HttpRequestMessage GetRenterProfile() => Get("v1/renters/profile");
    }
}