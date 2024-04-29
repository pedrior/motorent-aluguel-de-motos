namespace Motorent.Contracts.Auth.Responses;

public sealed record TokenResponse
{
    public string Type { get; init; } = null!;
    
    public string AccessToken { get; init; } = null!;
    
    public int ExpiresIn { get; init; }
}