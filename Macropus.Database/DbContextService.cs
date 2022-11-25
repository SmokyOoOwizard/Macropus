using Macropus.Database.Interfaces;
using Macropus.Database.Interfaces.Migration;
using Macropus.Database.Migration;
using Macropus.Database.Sqlite;

namespace Macropus.Database;

internal class DbContextService : IDbContextService
{
	public async Task<T> GetOrCreateDbContextAsync<T, TM>(
		string path,
		CancellationToken cancellationToken = default
	) where T : IBestDbContext, new() where TM : IMigrationsProvider
	{
		var dbProvider = new SqliteDbProvider($"Data Source={path}", path, Guid.Empty);

		await using (var dbConnection = dbProvider.CreateConnection())
		{
			await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

			if (DbUtils.GetTableCount(dbConnection) == 0)
				await DbUtils.MigrateDb<TM>(
						dbConnection, 0, TM.LastVersion, cancellationToken)
					.ConfigureAwait(false);
		}

		var dbContext = new T();
		dbContext.SetDbConnection(dbProvider.CreateConnection());

		return dbContext;
	}
}