using Macropus.Database.Adapter;
using Macropus.Database.Migration;
using System.Data;
using Macropus.Database.Interfaces.Migration;

namespace Macropus.FileSystem.Db.Migrations
{
	public sealed class m1_BaseFileSystemDbMigration : IMigration
	{
		public uint Version => 1;

		public async Task DownAsync(IDbConnection connection)
		{
			using var transaction = connection.BeginTransaction();
			try
			{
				var cmd = transaction.Connection.CreateCommand();
				cmd.CommandText = "DROP TABLE Files;";

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
					@"CREATE TABLE Files ("
				  + @"Id TEXT NOT NULL COLLATE NOCASE,"
				  + @"Name TEXT,"
				  + @"ObjectName TEXT NOT NULL"
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
}
