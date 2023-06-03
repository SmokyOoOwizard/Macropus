using System.Data;
using System.Text;
using ECS.Schema;
using ECS.Serialize.Extensions;
using Macropus.Database.Adapter;
using Microsoft.Data.Sqlite;
using SpanJson;

namespace ECS.Serialize.Serialize;

internal class SqlSerializer
{
	public async Task<int> InsertComponent(IDbConnection dbConnection, DataSchema schema, object obj)
	{
		var tableName = ComponentFormatUtils.NormalizeName(schema.SchemaOf.FullName);
		var fields = schema.Elements;

		if (tableName == null)
			throw new Exception(); // TODO

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
}