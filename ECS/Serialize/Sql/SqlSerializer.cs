﻿using System.Collections;
using System.Data;
using System.Text;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Macropus.ECS.Serialize.Extensions;
using Macropus.Linq;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Sql;

class SqlSerializer : IClearable
{
	private readonly StringBuilder sqlBuilder = new();

	public async Task<int> InsertComponent(IDbConnection dbConnection, SerializeState target)
	{
		var fields = target.Schema.Elements;

		var tableName = target.Schema.SchemaOf.FullName;

		sqlBuilder.Clear();
		sqlBuilder.Append($"INSERT INTO '{tableName}' (");
		sqlBuilder.Append(string.Join(',', fields.Select(e => e.Info.ToSqlName())));
		sqlBuilder.Append(") VALUES (");

		foreach (var element in fields)
		{
			var value = element.FieldInfo.GetValue(target.Value);
			if (element.Info.CollectionType is ECollectionType.Array)
			{
				InsertArray(target, element, value);
				continue;
			}

			if (element.Info.Type is ESchemaElementType.ComplexType)
				sqlBuilder.Append(element.ToSqlInsert(target.GetProcessed(element)?.First()));
			else
			{
				sqlBuilder.Append(element.ToSqlInsert(value));
			}
		}

		sqlBuilder.Remove(sqlBuilder.Length - 2, 2);

		sqlBuilder.Append("); select last_insert_rowid();");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		var reader = await cmd.ExecuteReaderAsync();

		await reader.ReadAsync();

		return reader.GetInt32(0);
	}

	private void InsertArray(SerializeState target, DataSchemaElement element, object? value)
	{
		IEnumerable? enumerable = null;
		if (element.Info.Type is ESchemaElementType.ComplexType)
		{
			var ids = target.GetProcessed(element);
			if (ids != null)
			{
				ids.Reverse();
				enumerable = ids;
			}
		}
		else
			enumerable = (value as IEnumerable)!;

		if (enumerable == null)
		{
			sqlBuilder.Append("null, ");
			return;
		}

		sqlBuilder.Append("json('[");

		if (enumerable.Any())
		{
			foreach (var collectionValue in enumerable)
				sqlBuilder.Append(element.ToSqlInsert(collectionValue).Replace('\'', '"'));

			sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
		}

		sqlBuilder.Append("]'), ");
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
		sqlBuilder.Append($"\'{entityId.ToString("N")}\'");
		sqlBuilder.Append(");");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		await cmd.ExecuteNonQueryAsync();
	}

	public void Clear()
	{
		sqlBuilder.Clear();
	}
}