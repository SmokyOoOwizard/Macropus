using System.Data;
using Macropus.Database.Adapter;
using Macropus.Database.Migration;

namespace Macropus.Database.Db.Migrations;

public sealed class m1_BaseDatabasesProviderDbMigration : IMigration
{
	public uint Version => 1;

	public async Task DownAsync(IDbConnection connection)
	{
		using var transaction = connection.BeginTransaction();
		try
		{
			var cmd = transaction.Connection.CreateCommand();
			cmd.CommandText = "DROP TABLE Databases;";

			await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	public async Task UpAsync(IDbConnection connection)
	{
		using var transaction = connection.BeginTransaction();
		try
		{
			var cmd = transaction.Connection.CreateCommand();
			cmd.CommandText =
				@"CREATE TABLE Databases ("
				+ @"Id TEXT NOT NULL COLLATE NOCASE,"
				+ @"Name TEXT NOT NULL,"
				+ @"FileId TEXT NOT NULL"
				+ @");";

			await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	public Task<bool> IsDbStateEqualsThisMigrationAsync(IDbConnection connection)
	{
		throw new NotImplementedException();
	}
}