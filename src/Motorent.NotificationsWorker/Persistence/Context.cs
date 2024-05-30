using System.Data;
using Npgsql;

namespace Motorent.NotificationsWorker.Persistence;

internal abstract class Context(string connectionString)
{
    public IDbConnection CreateConnection() => new NpgsqlConnection(connectionString);

    public virtual Task EnsureInitializedAsync() => Task.CompletedTask;
}