using Coravel;
using Microsoft.AspNetCore.Builder;
using Motorent.Infrastructure.Common.Jobs;
using Serilog;

namespace Motorent.Infrastructure;

public static class StartupExtensions
{
    public static void UseInfrastructure(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        
        app.UseAuthentication();
        
        app.UseAuthorization();

        app.Services.UseScheduler(scheduler =>
        {
            scheduler.Schedule<ProcessOutboxMessagesJob>()
                .EverySeconds(10)
                .PreventOverlapping(nameof(ProcessOutboxMessagesJob));
        });
    }
}