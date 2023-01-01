using System.Data;
using System.Text;
using Macropus.ECS.Serialize.Extensions;
using Macropus.Schema;
using Microsoft.Data.Sqlite;

namespace Macropus.ECS.Serialize;

public static class DbCommandCache
{
	private static readonly Dictionary<IDbConnection, Dictionary<string, IDbCommand>> Exists = new();
	private static readonly StringBuilder SqlBuilder = new();

	public static IDbCommand GetReadCmd(
		IDbConnection dbConnection,
		string tableName,
		IReadOnlyCollection<DataSchemaElement> fields,
		int count = 1
	)
	{
		if (!Exists.TryGetValue(dbConnection, out var existsCmd))
		{
			Exists[dbConnection] = existsCmd = new();
		}

		var cmdName = "GET_" + tableName + "_" + count;

		if (!existsCmd.TryGetValue(cmdName, out var cmd))
		{
			cmd = dbConnection.CreateCommand();

			SqlBuilder.Clear();
			SqlBuilder.Append("SELECT ");
			SqlBuilder.Append(string.Join(',', fields.Select(e => e.Info.ToSqlName())));
			SqlBuilder.Append($" FROM '{tableName}' WHERE Id in (@id");

			for (int i = 1; i < count; i++)
				SqlBuilder.Append($", @id{i}");

			SqlBuilder.Append(");");

			cmd.CommandText = SqlBuilder.ToString();


			existsCmd[cmdName] = cmd;
		}

		cmd.Parameters.Clear();

		return cmd;
	}

	public static IDbCommand GetWWCmd(IDbConnection dbConnection, Guid entityId, string componentName)
	{
		if (!Exists.TryGetValue(dbConnection, out var existsCmd))
		{
			Exists[dbConnection] = existsCmd = new();
		}

		const string cmdName = "WW_" + ComponentSerializer.ENTITIES_COMPONENTS_TABLE_NAME;

		if (!existsCmd.TryGetValue(cmdName, out var cmd))
		{
			cmd = dbConnection.CreateCommand();

			SqlBuilder.Clear();
			SqlBuilder.Append(
				$"SELECT (ComponentId) FROM '{ComponentSerializer.ENTITIES_COMPONENTS_TABLE_NAME}' WHERE EntityId = @entityId AND ComponentName = @componentName;");

			cmd.CommandText = SqlBuilder.ToString();


			existsCmd[cmdName] = cmd;
		}

		cmd.Parameters.Clear();
		cmd.Parameters.Add(new SqliteParameter("@entityId", entityId.ToString("N")));
		cmd.Parameters.Add(new SqliteParameter("@componentName", componentName));

		return cmd;
	}

	public static void Clear(IDbConnection dbConnection)
	{
		if (Exists.TryGetValue(dbConnection, out var cmds))
		{
			foreach (var (_, cmd) in cmds)
				cmd.Dispose();
		}

		Exists.Remove(dbConnection);
	}
}