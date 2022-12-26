using System.Collections;
using System.Data;
using System.Text;
using AnyOfTypes;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Macropus.ECS.Serialize.Extensions;
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
		IReadOnlyCollection<DataSchemaElement> fields
	)
	{
		if (existsCmd.TryGetValue(tableName, out var cmd))
			return cmd;

		cmd = dbConnection.CreateCommand();

		sqlBuilder.Clear();
		sqlBuilder.Append($"INSERT INTO '{tableName}' (");
		sqlBuilder.Append(string.Join(',', fields.Select(e => e.Info.ToSqlName())));
		sqlBuilder.Append(") VALUES(");

		foreach (var element in fields)
		{
			sqlBuilder.Append($"@{element.Info.FieldName}, ");
		}

		sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
		sqlBuilder.Append(") RETURNING Id;");

		cmd.CommandText = sqlBuilder.ToString();

		existsCmd[tableName] = cmd;

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

		var cmd = GetCmd(dbConnection, tableName, fields);

		count = Math.Min(count, values.Count);

		List<long?> ids = new();
		for (var i = 0; i < count; i++)
		{
			var value = values.Dequeue();
			if (value == null)
			{
				ids.Add(null);
				continue;
			}

			cmd.Parameters.Clear();

			FillCmd(cmd, fields, value);

			using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
			await reader.ReadAsync().ConfigureAwait(false);

			ids.Add(reader.GetInt32(0));
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

		var cmd = GetCmd(dbConnection, tableName, fields);
		cmd.Parameters.Clear();

		FillCmd(cmd, fields, target);

		using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

		await reader.ReadAsync().ConfigureAwait(false);

		return reader.GetInt32(0);
	}

	private void FillCmd(
		IDbCommand cmd,
		IReadOnlyCollection<DataSchemaElement> fields,
		AnyOf<object, ComponentSerializeState> value
	)
	{
		foreach (var element in fields)
		{
			object? fieldValue;

			if (value.IsFirst)
				fieldValue = element.FieldInfo.GetValue(value.First);
			else
				fieldValue = element.FieldInfo.GetValue(value.Second.Value);

			object? valueToInsert;

			if (element.Info.CollectionType is ECollectionType.Array)
			{
				if (value.IsFirst)
					valueToInsert = InsertArray(element, fieldValue as IEnumerable);
				else
					valueToInsert = InsertArray(value.Second, element, fieldValue);
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

			if (valueToInsert == null)
				cmd.Parameters.Add(new SqliteParameter($"@{element.Info.FieldName}", DBNull.Value));
			else
				cmd.Parameters.Add(new SqliteParameter($"@{element.Info.FieldName}", valueToInsert));
		}
	}

	private string? InsertArray(ISerializeState state, DataSchemaElement element, object? value)
	{
		IEnumerable? enumerable = null;
		if (element.Info.Type is ESchemaElementType.ComplexType)
		{
			if (state is not ComponentSerializeState css)
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