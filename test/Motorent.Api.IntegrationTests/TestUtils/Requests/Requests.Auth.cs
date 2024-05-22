using Motorent.Contracts.Auth.Requests;

namespace Motorent.Api.IntegrationTests.TestUtils.Requests;

internal static partial class Requests
{
    public static class Auth
    {
        public static HttpRequestMessage Login(LoginRequest request) => Post("v1/auth/login", request);
        
        public static HttpRequestMessage Register(RegisterRequest request) => Post("v1/auth/register", request);
    }
}