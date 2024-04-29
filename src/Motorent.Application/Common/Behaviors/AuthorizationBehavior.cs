using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Motorent.Application.Common.Abstractions.Security;

namespace Motorent.Application.Common.Behaviors;

internal sealed class AuthorizationBehavior<TRequest, TResponse>(
    IServiceProvider serviceProvider,
    ILogger<AuthorizationBehavior<TRequest, TResponse>> logger,
    IAuthorizer<TRequest>? authorizer = null)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IBaseRequest where TResponse : IResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (authorizer is null)
        {
            // A requisição não requer autorização
            return await next();
        }
        
        var requestName = request.GetType().Name;

        foreach (var requirement in authorizer.GetRequirements(request))
        {
            var handler = ResolveAuthorizationRequirementHandler(requirement.GetType(), serviceProvider);
            var authorized = await handler.AuthorizeAsync(requirement, cancellationToken);
            if (authorized)
            {
                continue;
            }

            logger.LogInformation("Request {RequestName} does not meet the authorization requirement {RequirementName}",
                requestName, requirement.GetType().Name);

            return (TResponse)(dynamic)Error.Forbidden("You are not authorized to perform this action.");
        }

        return await next();
    }

    private static IRequirementHandler ResolveAuthorizationRequirementHandler(Type requirementType,
        IServiceProvider serviceProvider)
    {
        var requirementHandler = typeof(IRequirementHandler<>).MakeGenericType(requirementType);
        return serviceProvider.GetRequiredService(requirementHandler) as IRequirementHandler
               ?? throw new InvalidOperationException(
                   $"No authorization requirement handler found for '{requirementType.Name}'");
    }
}