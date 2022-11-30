using System.Data;

namespace Macropus.Database.Interfaces;

public interface IDatabasesService : IDisposable
{
	Task<IDbConnection?> TryGetDatabase(string name, CancellationToken cancellationToken = default);
	Task<bool> CreateDatabaseAsync(string name, CancellationToken cancellationToken = default);
	Task<bool> DeleteDatabaseAsync(string name, CancellationToken cancellationToken = default);
}