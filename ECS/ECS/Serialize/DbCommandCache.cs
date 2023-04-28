using System.Data;
using System.Text;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.ECS.Serialize.Extensions;
using Macropus.Schema;
using Microsoft.Data.Sqlite;

namespace Macropus.ECS.Serialize;

public static class DbCommandCache
{
	private static readonly Dictionary<IDbConnection, Dictionary<string, IDbCommand>> Exists = new();
	private static readonly StringBuilderPool SbPool = StringBuilderPool.Instance; 

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

			var sqlBuilder = SbPool.Take();
			sqlBuilder.Append("SELECT ");
			sqlBuilder.Append(string.Join(',', fields.Select(e => e.Info.ToSqlName())));
			sqlBuilder.Append($" FROM '{tableName}' WHERE Id in (@id");

			for (int i = 1; i < count; i++)
				sqlBuilder.Append($", @id{i}");

			sqlBuilder.Append(");");

			cmd.CommandText = sqlBuilder.ToString();

			SbPool.Release(sqlBuilder);

			existsCmd[cmdName] = cmd;
		}

		cmd.Parameters.Clear();

		return cmd;
	}

	public static IDbCommand GetComponentIdCmd(IDbConnection dbConnection, Guid entityId, string componentName)
	{
		if (!Exists.TryGetValue(dbConnection, out var existsCmd))
		{
			Exists[dbConnection] = existsCmd = new();
		}

		const string cmdName = "GetComponentId_" + ComponentSerializer.ENTITIES_COMPONENTS_TABLE_NAME;

		if (!existsCmd.TryGetValue(cmdName, out var cmd))
		{
			cmd = dbConnection.CreateCommand();

			var sqlBuilder = SbPool.Take();
			sqlBuilder.Append(
				$"SELECT (ComponentId) FROM '{ComponentSerializer.ENTITIES_COMPONENTS_TABLE_NAME}' WHERE EntityId = @entityId AND ComponentName = @componentName;");

			cmd.CommandText = sqlBuilder.ToString();

			SbPool.Release(sqlBuilder);

			existsCmd[cmdName] = cmd;
		}

		cmd.Parameters.Clear();
		cmd.Parameters.Add(new SqliteParameter("@entityId", entityId.ToString("N")));
		cmd.Parameters.Add(new SqliteParameter("@componentName", componentName));

		return cmd;
	}
	
	public static IDbCommand GetInsertCmd(
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

			var sqlBuilder = SbPool.Take();
			sqlBuilder.Append($"INSERT INTO '{tableName}' (");
			sqlBuilder.Append(string.Join(',', fields.Select(e => e.Info.ToSqlName())));
			sqlBuilder.Append(") VALUES ");

			for (var i = 0; i < count; i++)
			{
				sqlBuilder.Append('(');
				foreach (var element in fields)
				{
					sqlBuilder.Append($"@{i}_{element.Info.FieldName}, ");
				}

				sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
				sqlBuilder.Append("), ");
			}

			sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
			sqlBuilder.Append(" RETURNING Id;");

			cmd.CommandText = sqlBuilder.ToString();

			SbPool.Release(sqlBuilder);

			existsCmd[cmdName] = cmd;
		}
		
		cmd.Parameters.Clear();

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