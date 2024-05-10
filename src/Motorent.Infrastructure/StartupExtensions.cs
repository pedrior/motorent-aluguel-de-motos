using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        app.UseHangfireDashboard();

        app.UseBackgroundJobs();
    }

    private static void UseHangfireDashboard(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = []
            });
        }
    }

    private static void UseBackgroundJobs(this WebApplication app)
    {
        var jobManager = app.Services.GetRequiredService<IRecurringJobManager>();
        
        jobManager.AddOrUpdate<IProcessOutboxMessagesJob>(
            recurringJobId: "process-outbox-messages",
            methodCall: job => job.ProcessAsync(),
            cronExpression: "0/15 * * * * *");
    }
}