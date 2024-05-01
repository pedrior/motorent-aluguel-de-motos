using Motorent.Application.Motorcycles.Register;
using Motorent.Contracts.Motorcycles.Requests;

namespace Motorent.Presentation.Motorcycles;

internal sealed class Register : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("motorcycles", (
                RegisterMotorcycleRequest request,
                ISender sender,
                CancellationToken cancellationToken) => sender.Send(
                    request.Adapt<RegisterMotorcycleCommand>(),
                    cancellationToken)
                .ToResponseAsync(response => Results.Created(
                    uri: null as Uri,
                    value: response)))
            .AllowAnonymous();
    }
}