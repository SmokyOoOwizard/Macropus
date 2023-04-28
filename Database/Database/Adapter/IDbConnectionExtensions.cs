using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace Macropus.Database.Adapter;

public static class IDbConnectionExtensions
{
    private static readonly ConcurrentDictionary<Type, ConnectionAdapter> ConnectionAdapters = new();

    public static Task OpenAsync(this IDbConnection connection)
    {
        if (connection == null) throw new ArgumentNullException(nameof(connection));
        var dbConnection = connection as DbConnection;
        if (dbConnection != null) return dbConnection.OpenAsync();
        return ConnectionAdapters.GetOrAdd(connection.GetType(),
                type => new ConnectionAdapter(type))
            .OpenAsync(connection);
    }

    public static Task OpenAsync(this IDbConnection connection, CancellationToken token)
    {
        if (connection == null) throw new ArgumentNullException(nameof(connection));
        var dbConnection = connection as DbConnection;
        if (dbConnection != null) return dbConnection.OpenAsync(token);
        return ConnectionAdapters.GetOrAdd(connection.GetType(),
                type => new ConnectionAdapter(type))
            .OpenAsyncToken(connection, token);
    }
}