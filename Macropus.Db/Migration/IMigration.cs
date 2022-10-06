using System.Data;

namespace Macropus.Db.Migration;

public interface IMigration
{
    uint Version { get; }

    Task<bool> IsDbStateEqualsThisMigrationAsync(IDbConnection connection);
    Task UpAsync(IDbConnection connection);
    Task DownAsync(IDbConnection connection);
}