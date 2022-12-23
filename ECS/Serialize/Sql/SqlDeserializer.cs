using System.Data;
using System.Text;
using System.Text.Json.Nodes;
using Macropus.CoolStuff;
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

		for (int i = 0; i < unreadValues.Count; i++)
		{
			var element = unreadValues[i];

			var elementType = element.Info.Type;
			if (element.Info.CollectionType is ECollectionType.Array)
			{
				if (reader.IsDBNull(i))
				{
					state.ReadValues.Add(new(element, null));
					continue;
				}

				var rawArray = reader.GetString(i);

				var obj = JsonNode.Parse(rawArray)!.AsArray();

				if (elementType == ESchemaElementType.ComplexType)
				{
					if (obj.Count == 0)
					{
						var w = Array.CreateInstance(element.FieldInfo.FieldType.GetElementType(), obj.Count);
						state.ReadValues.Add(new(element, w));
						continue;
					}

					for (int j = 0; j < obj.Count; j++)
					{
						var r = obj[j]?.ToString();

						if (string.IsNullOrWhiteSpace(r))
						{
							state.AddRef(element, null);
						}
						else
						{
							state.AddRef(element, long.Parse(r));
						}
					}

					continue;
				}

				var array = Array.CreateInstance(element.ToType(), obj.Count);

				for (int j = 0; j < obj.Count; j++)
				{
					array.SetValue(element.Info.Parse(obj[j]?.ToString()), j);
				}

				state.ReadValues.Add(new(element, array));
				continue;
			}

			if (elementType == ESchemaElementType.ComplexType)
			{
				state.AddRef(element, reader.GetInt64(i));
				continue;
			}

			object? value = reader.IsDBNull(i) ? null : elementType.Read(reader, i);
			state.ReadValues.Add(new(element, value));
		}

		state.MarkUnreadFieldsAsRead();
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

		return (long)await cmd.ExecuteScalarAsync();
	}

	public void Clear()
	{
		sqlBuilder.Clear();
	}
}