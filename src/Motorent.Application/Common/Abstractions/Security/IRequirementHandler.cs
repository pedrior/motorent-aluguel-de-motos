namespace Motorent.Application.Common.Abstractions.Security;

internal interface IRequirementHandler
{
    Task<bool> AuthorizeAsync(object requirement, CancellationToken cancellationToken);
}

internal interface IRequirementHandler<in TRequirement> : IRequirementHandler where TRequirement : IRequirement
{
    Task<bool> AuthorizeAsync(TRequirement requirement, CancellationToken cancellationToken);

    Task<bool> IRequirementHandler.AuthorizeAsync(object requirement, CancellationToken cancellationToken) => 
        AuthorizeAsync((TRequirement)requirement, cancellationToken);
}