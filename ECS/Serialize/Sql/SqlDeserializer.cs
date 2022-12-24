using System.Data;
using System.Text;
using System.Text.Json.Nodes;
using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections;
using Macropus.Database.Adapter;
using Macropus.ECS.Serialize.Extensions;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Sql;

class SqlDeserializer : IClearable
{
	private readonly StringBuilder sqlBuilder = new();

	public async Task ReadComponentPart(IDbConnection dbConnection, DeserializeState state)
	{
		var unreadValues = state.GetUnreadFields();
		var elements = unreadValues.ToSqlName();
		var tableName = state.Schema.SchemaOf.FullName;

		sqlBuilder.Clear();
		sqlBuilder.Append($"SELECT {string.Join(',', elements)} FROM '{tableName}' ");
		sqlBuilder.Append($"WHERE Id = '{state.ComponentId}'");
		sqlBuilder.Append(';');

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		var reader = await cmd.ExecuteReaderAsync();

		if (!await reader.ReadAsync())
		{
			// TODO we can't find component with this id
			throw new Exception();
		}

		ProcessUnreadedValues(state, unreadValues, reader);

		state.MarkUnreadFieldsAsRead();
	}

	private static void ProcessUnreadedValues(
		DeserializeState state,
		IReadOnlyList<DataSchemaElement> unreadValues,
		IDataReader reader
	)
	{
		for (int i = 0; i < unreadValues.Count; i++)
		{
			var element = unreadValues[i];

			if (reader.IsDBNull(i))
			{
				state.ReadValues.Add(new(element, null));
				continue;
			}

			var elementType = element.Info.Type;
			if (element.Info.CollectionType is ECollectionType.Array)
			{
				if (elementType == ESchemaElementType.ComplexType)
					ReadComplexArray(state, reader, i, element);
				else
					ReadSimpleArray(state, reader, i, element);

				continue;
			}

			if (elementType == ESchemaElementType.ComplexType)
			{
				state.AddRef(element, reader.GetInt64(i));
				continue;
			}

			object value = elementType.Read(reader, i);
			state.ReadValues.Add(new(element, value));
		}
	}

	private static void ReadSimpleArray(
		DeserializeState state,
		IDataReader reader,
		int i,
		DataSchemaElement element
	)
	{
		var rawArray = reader.GetString(i);
		var jsonArray = JsonNode.Parse(rawArray)!.AsArray();

		var fieldType = element.FieldInfo.FieldType.GetElementType();
		if (fieldType == null)
			// TODO
			throw new Exception();

		var array = Array.CreateInstance(fieldType, jsonArray.Count);

		for (var j = 0; j < array.Length; j++)
			array.SetValue(element.Info.Parse(jsonArray[j]?.ToString()), j);

		state.ReadValues.Add(new(element, array));
	}

	private static void ReadComplexArray(
		DeserializeState state,
		IDataReader reader,
		int i,
		DataSchemaElement element
	)
	{
		var rawArray = reader.GetString(i);
		var jsonArray = JsonNode.Parse(rawArray)!.AsArray();

		for (var j = 0; j < jsonArray.Count; j++)
		{
			var r = jsonArray[j]?.ToString();

			if (string.IsNullOrWhiteSpace(r))
				state.AddRef(element, null);
			else
				state.AddRef(element, long.Parse(r));
		}

		if (jsonArray.Count > 0)
			return;

		var fieldType = element.FieldInfo.FieldType.GetElementType();
		if (fieldType == null)
			// TODO
			throw new Exception();
		
		var array = ArrayExtensions.Empty(fieldType);

		state.ReadValues.Add(new(element, array));
	}

	public async Task<long> GetComponentIdAsync(IDbConnection dbConnection, Guid entityId, string componentName)
	{
		sqlBuilder.Clear();
		sqlBuilder.Append(
			$"SELECT (ComponentId) FROM '{ComponentSerializer.ENTITIES_COMPONENTS_TABLE_NAME}' ");
		sqlBuilder.Append($"WHERE EntityId = '{entityId:N}'");
		sqlBuilder.Append(" AND ");
		sqlBuilder.Append($"ComponentName = '{componentName}'");
		sqlBuilder.Append(';');

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();
		
		var rawId = await cmd.ExecuteScalarAsync();
		if (rawId is not long id)
			// TODO
			throw new Exception();

		return id;
	}

	public void Clear()
	{
		sqlBuilder.Clear();
	}
}