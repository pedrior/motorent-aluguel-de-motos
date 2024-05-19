using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(AuthEndpoints))]
public sealed partial class AuthEndpointsTests(WebApplicationFactory api) : WebApplicationFixture(api);