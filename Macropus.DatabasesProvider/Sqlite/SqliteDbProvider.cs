using System.Data.Common;
using Macropus.Database;
using Microsoft.Data.Sqlite;

namespace Macropus.DatabasesProvider.Sqlite;

internal sealed class SqliteDbProvider : IDbProvider
{
	public Guid Id { get; }

	public string Name { get; }

	public uint Version { get; private set; }

	private readonly string connectionString;

	public SqliteDbProvider(string connectionString, string name, Guid id)
	{
		Id = id;
		Name = name;
		this.connectionString = connectionString;

		RefreshVersion();
	}

	public void RefreshVersion()
	{
		using var connection = CreateConnection();
		connection.Open();

		Version = DbUtils.GetVersion(connection);
	}

	public DbConnection CreateConnection()
	{
		return new SqliteConnection(connectionString);
	}
}