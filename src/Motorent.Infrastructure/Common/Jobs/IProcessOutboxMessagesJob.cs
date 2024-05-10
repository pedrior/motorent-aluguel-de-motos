namespace Motorent.Infrastructure.Common.Jobs;

internal interface IProcessOutboxMessagesJob
{
    Task ProcessAsync();
}