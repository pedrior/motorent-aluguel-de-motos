using Motorent.Api;
using Motorent.Application;
using Motorent.Infrastructure;
using Motorent.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation()
    .AddApi();

var app = builder.Build();

app.UsePresentation();

app.UseInfrastructure();

app.UseApi();

app.Run();

namespace Motorent.Api
{
    public partial class Program;
}