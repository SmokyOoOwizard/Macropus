using System.Collections;
using System.Data;
using System.Text;
using AnyOfTypes;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Macropus.ECS.Serialize.Extensions;
using Macropus.ECS.Serialize.Serialize;
using Macropus.Linq;
using Macropus.Schema;
using Microsoft.Data.Sqlite;

namespace Macropus.ECS.Serialize.Sql;

class SqlSerializer : IClearable
{
	private readonly StringBuilder sqlBuilder = new();
	private readonly Dictionary<string, IDbCommand> existsCmd = new();

	private IDbCommand GetCmd(
		IDbConnection dbConnection,
		string tableName,
		IReadOnlyCollection<DataSchemaElement> fields,
		int count = 1
	)
	{
		if (existsCmd.TryGetValue(tableName + count, out var cmd))
			return cmd;

		cmd = dbConnection.CreateCommand();

		sqlBuilder.Clear();
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

		existsCmd[tableName + count] = cmd;

		return cmd;
	}


	public async Task<long?[]> InsertComponent(IDbConnection dbConnection, ParallelSerializeState target, int count)
	{
		if (target.Schema == null)
			// TODO
			throw new Exception();

		var values = target.Values;
		if (values == null)
			throw new Exception();

		var tableName = target.Schema.SchemaOf.FullName;
		var fields = target.Schema.Elements;
		
		if (tableName == null)
			// TODO
			throw new Exception();

		count = Math.Min(count, values.Count);

		var notNullCount = values.Take(count).Count(w => w != null);
		if (notNullCount == 0)
		{
			values.Clear();
			return new long?[count];
		}

		var cmd = GetCmd(dbConnection, tableName, fields, notNullCount);
		cmd.Parameters.Clear();

		var ids = new List<long?>();
		var y = 0;
		for (var i = 0; i < count; i++)
		{
			var value = values.Dequeue();
			if (value == null)
			{
				ids.Add(null);
				continue;
			}

			FillCmd(cmd, fields, value, y + "_");
			y++;

			ids.Add(0);
		}

		using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
		await reader.ReadAsync().ConfigureAwait(false);

		for (var i = 0; i < count; i++)
		{
			if (ids[i] == null)
				continue;

			ids[i] = reader.GetInt32(0);
			await reader.ReadAsync().ConfigureAwait(false);
		}

		return ids.ToArray();
	}


	public async Task<int> InsertComponent(IDbConnection dbConnection, ComponentSerializeState target)
	{
		if (target.Schema == null)
			// TODO
			throw new Exception();

		var tableName = target.Schema.SchemaOf.FullName;
		var fields = target.Schema.Elements;

		if (tableName == null)
			// TODO
			throw new Exception();

		var cmd = GetCmd(dbConnection, tableName, fields);
		cmd.Parameters.Clear();

		FillCmd(cmd, fields, target, "0_");

		using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

		await reader.ReadAsync().ConfigureAwait(false);

		return reader.GetInt32(0);
	}

	private void FillCmd(
		IDbCommand cmd,
		IReadOnlyCollection<DataSchemaElement> fields,
		AnyOf<object, ComponentSerializeState> value,
		string prefix = ""
	)
	{
		foreach (var element in fields)
		{
			var fieldValue = element.FieldInfo.GetValue(value.IsFirst ? value.First : value.Second.Value);

			object? valueToInsert;
			if (element.Info.CollectionType is ECollectionType.Array)
			{
				valueToInsert = value.IsFirst
					? InsertArray(element, fieldValue as IEnumerable)
					: InsertArray(value.Second, element, fieldValue);
			}
			else if (element.Info.Type is ESchemaElementType.ComplexType)
			{
				if (value.IsFirst)
					// TODO
					throw new Exception();

				valueToInsert = value.Second.GetProcessed(element)?.First();
			}
			else
				valueToInsert = element.ToSqlInsert(fieldValue);

			cmd.Parameters.Add(new SqliteParameter($"@{prefix}{element.Info.FieldName}",
				valueToInsert ?? DBNull.Value));
		}
	}

	private string? InsertArray(ISerializeState state, DataSchemaElement element, object? value)
	{
		IEnumerable? enumerable = null;
		if (element.Info.Type is ESchemaElementType.ComplexType)
		{
			if (state is not ComponentSerializeState css)
				// TODO
				throw new Exception();

			var ids = css.GetProcessed(element);
			if (ids != null)
			{
				ids.Reverse();
				enumerable = ids;
			}
		}
		else
			enumerable = (value as IEnumerable)!;

		return InsertArray(element, enumerable);
	}

	private string? InsertArray(DataSchemaElement element, IEnumerable? enumerable)
	{
		if (enumerable == null)
			return null;

		sqlBuilder.Clear();
		sqlBuilder.Append('[');

		// ReSharper disable once PossibleMultipleEnumeration
		if (enumerable.Any())
		{
			// ReSharper disable once PossibleMultipleEnumeration
			foreach (var collectionValue in enumerable)
				sqlBuilder.Append(element.ToSqlArrayInsert(collectionValue).Replace('\'', '"'));

			sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
		}

		sqlBuilder.Append(']');
		return sqlBuilder.ToString();
	}

	public async Task AddEntityComponent(
		IDbConnection dbConnection,
		long componentId,
		string componentName,
		Guid entityId
	)
	{
		sqlBuilder.Clear();
		sqlBuilder.Append(
			$"INSERT INTO '{ComponentSerializer.ENTITIES_COMPONENTS_TABLE_NAME}' (ComponentId, ComponentName, EntityId) VALUES(");
		sqlBuilder.Append($"{componentId}, ");
		sqlBuilder.Append($"'{componentName}', ");
		sqlBuilder.Append($"\'{entityId:N}\'");
		sqlBuilder.Append(");");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
	}

	public void Clear()
	{
		foreach (var dbCommand in existsCmd)
		{
			dbCommand.Value.Dispose();
		}

		existsCmd.Clear();
		sqlBuilder.Clear();
	}
}