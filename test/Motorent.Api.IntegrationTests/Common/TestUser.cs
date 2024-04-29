using Microsoft.IdentityModel.JsonWebTokens;

namespace Motorent.Api.IntegrationTests.Common;

public sealed record TestUser(
    string Email,
    string Password,
    string[]? Roles = null,
    IDictionary<string, string>? Claims = null)
{
    private static readonly TestUser Default = new(
        Email: "john@doe.com",
        Password: "JohnDoe123",
        Claims: new Dictionary<string, string>
        {
            [JwtRegisteredClaimNames.GivenName] = "John",
            [JwtRegisteredClaimNames.FamilyName] = "Doe",
            [JwtRegisteredClaimNames.Birthdate] = "2000-09-05"
        });

    public static TestUser Admin => Default with { Roles = ["admin"] };
    
    public static TestUser Renter => Default with { Roles = ["renter"] };
}