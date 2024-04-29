namespace Motorent.Application.Common.Abstractions.Security;

internal interface IAuthorizer<in T> where T : class
{
    IEnumerable<IRequirement> GetRequirements(T subject);
}