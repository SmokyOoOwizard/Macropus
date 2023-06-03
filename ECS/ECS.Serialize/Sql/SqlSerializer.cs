using System.Data;
using System.Text;
using ECS.Schema;
using ECS.Serialize.Extensions;
using ECS.Serialize.Models;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Microsoft.Data.Sqlite;
using SpanJson;

namespace ECS.Serialize.Sql;

class SqlSerializer : IClearable
{
	private readonly StringBuilder sqlBuilder = new();

	public async Task<int> InsertComponent(IDbConnection dbConnection, DataSchema schema, object obj)
	{

		var tableName = ComponentFormatUtils.NormalizeName(schema.SchemaOf.FullName);
		var fields = schema.Elements;

		if (tableName == null)
			// TODO
			throw new Exception();

		var cmd = DbCommandCache.GetInsertCmd(dbConnection, tableName, fields);

		FillCmd(cmd, fields, obj, "0_");

		using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

		await reader.ReadAsync().ConfigureAwait(false);

		return reader.GetInt32(0);
	}

	private void FillCmd(
		IDbCommand cmd,
		IReadOnlyCollection<DataSchemaElement> fields,
		object value,
		string prefix = ""
	)
	{
		foreach (var element in fields)
		{
			var fieldValue = element.FieldInfo.GetValue(value);

			object? valueToInsert = null;
			if (element.Info.CollectionType is ECollectionType.Array || element.Info.Type is ESchemaElementType.ComplexType)
			{
				if (fieldValue != null)
				{
					var json = JsonSerializer.NonGeneric.Utf8.Serialize(fieldValue);
					valueToInsert = Encoding.UTF8.GetString(json);
				}
			}
			else
				valueToInsert = element.ToSqlInsert(fieldValue);

			cmd.Parameters.Add(new SqliteParameter($"@{prefix}{element.Info.FieldName}",
				valueToInsert ?? DBNull.Value));
		}
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
			$"INSERT INTO '{EntitiesComponentsTable.TABLE_NAME}' (ComponentId, ComponentName, EntityId) VALUES(");
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
		sqlBuilder.Clear();
	}
}