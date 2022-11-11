using System.Data;
using Macropus.Database.Adapter;

namespace Macropus.Database.Extensions;

public static class DbConnectionExtensions
{
	public static async Task<bool> TableAlreadyExists(this IDbConnection openConnection, string tableName)
	{
		if (openConnection.State != ConnectionState.Open)
			throw new ArgumentException("Data.ConnectionState must be open");

		var sql = "SELECT name FROM sqlite_master WHERE type='table' AND name='" + tableName + "';";

		using var cmd = openConnection.CreateCommand();
		cmd.CommandText = sql;

		using var reader = await cmd.ExecuteReaderAsync();

		return await reader.ReadAsync();
	}
}