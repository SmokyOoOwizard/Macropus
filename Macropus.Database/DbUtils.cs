using System.Data;
using Macropus.Database.Migration;

namespace Macropus.Database;

public static class DbUtils
{
    public static async Task MigrateDb<T>(IDbConnection connection, uint from, uint to,
        CancellationToken cancellationToken = default) where T : IMigrationsProvider
    {
        if (from == to)
            // TODO throw
            throw new Exception();

        var migrations = T.GetMigrations().Skip((int)Math.Min(from, to)).Take(Math.Abs((int)to - (int)from))
            .ToArray();
        if (to < from)
            migrations = migrations.Reverse().ToArray();


        // TODO possibly headache if while migrate throw exception. Transaction?
        for (var i = 0; i < migrations.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var migration = migrations[i];
            if (to < from)
                await migration.DownAsync(connection).ConfigureAwait(false);
            else
                await migration.UpAsync(connection).ConfigureAwait(false);
        }
    }

    public static uint GetVersion(IDbConnection connection)
    {
        using var cmd = connection.CreateCommand();

        cmd.CommandText = "PRAGMA user_version";

        var reader = cmd.ExecuteReader();

        reader.Read();

        return (uint)reader.GetInt32(0);
    }

    public static void SetVersion(IDbConnection connection, uint version)
    {
        using var cmd = connection.CreateCommand();

        cmd.CommandText = $"PRAGMA user_version={version}";

        cmd.ExecuteNonQuery();
    }

    public static int GetTableCount(IDbConnection connection)
    {
        using var cmd = connection.CreateCommand();

        cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master AS TABLES WHERE TYPE = 'table'";

        var reader = cmd.ExecuteReader();

        reader.Read();

        return reader.GetInt32(0);
    }
}