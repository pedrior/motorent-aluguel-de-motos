using Motorent.Contracts.Auth.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Auth
    {
        public static readonly LoginRequest LoginRequest = new()
        {
            Email = "john@doe.com",
            Password = "JohnDoe123"
        };

        public static readonly RegisterRequest RegisterRequest = new()
        {
            Email = "john@doe.com",
            Password = "JohnDoe123",
            GivenName = "John",
            FamilyName = "Doe",
            Birthdate = new DateOnly(2000, 09, 05),
            CNPJ = "18.864.014/0001-19",
            CNHNumber = "92353762700",
            CNHCategory = "ab",
            CNHExpDate = new DateOnly(DateTime.Today.Year + 1, 01, 01)
        };

        public static HttpRequestMessage Login(LoginRequest? request = null) =>
            Post("v1/auth/login", request ?? LoginRequest);
        
        public static HttpRequestMessage Register(RegisterRequest? request = null) =>
            Post("v1/auth/register", request ?? RegisterRequest);
    }
}