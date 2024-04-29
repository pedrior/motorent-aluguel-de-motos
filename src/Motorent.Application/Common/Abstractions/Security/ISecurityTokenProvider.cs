namespace Motorent.Application.Common.Abstractions.Security;

public interface ISecurityTokenProvider
{
    Task<SecurityToken> GenerateSecurityTokenAsync(string userId, CancellationToken cancellationToken = default);
}