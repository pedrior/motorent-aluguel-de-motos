using System.Data;

namespace Motorent.NotificationsWorker.Persistence;

internal sealed class DataContext(string connectionString) : Context(connectionString)
{
    public override Task EnsureInitializedAsync() => InitializeTablesAsync();

    private async Task InitializeTablesAsync()
    {
        using var connection = CreateConnection();
        await CreateNotificationsTableAsync(connection);
    }

    private static async Task CreateNotificationsTableAsync(IDbConnection connection)
    {
        const string sql = $"""
                            CREATE TABLE IF NOT EXISTS {TableNames.Notifications} (
                                id VARCHAR(36) PRIMARY KEY,
                                message VARCHAR(16384) NOT NULL,
                                created_at TIMESTAMP WITH TIME ZONE NOT NULL);
                            """;

        await connection.ExecuteAsync(sql);
    }
}