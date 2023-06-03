using System.Data;
using LinqToDB.Data;
using Macropus.Database.Adapter;

namespace Macropus.Database.Extensions;

public static class DbConnectionExtensions
{
	public static async Task<bool> TableAlreadyExists(this DataConnection openConnection, string tableName)
	{
		if (openConnection.Connection.State != ConnectionState.Open)
			throw new ArgumentException("Data.ConnectionState must be open");

		var sql = "SELECT name FROM sqlite_master WHERE type='table' AND name='" + tableName + "';";

		await using var cmd = openConnection.CreateCommand();
		cmd.CommandText = sql;

		await using var reader = await cmd.ExecuteReaderAsync();

		return await reader.ReadAsync();
	}
}