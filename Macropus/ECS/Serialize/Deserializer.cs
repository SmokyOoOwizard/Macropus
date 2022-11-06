using System.Data;
using System.Text;
using System.Text.Json.Nodes;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Macropus.ECS.Component;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class Deserializer : IClearable
{
	private readonly Stack<DeserializeState> deserializeStack = new();
	private readonly StringBuilder sqlBuilder = new();

	public async Task<T?> DeserializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId)
		where T : struct, IComponent
	{
		var componentName = schema.SchemaOf.FullName;

		var rootComponentId = await GetComponentIdAsync(dbConnection, entityId, componentName!);
		T rootComponent = default;

		deserializeStack.Push(new DeserializeState(schema, rootComponentId));


		do
		{
			var target = deserializeStack.Peek();

			if (target.UnreadValues.Count > 0)
				await ReadComponentPart(dbConnection, target);

			if (target.Refs.Count > 0)
			{
				var targetRef = target.Refs.Pop();
				var refSchema = schema.SubSchemas[targetRef.Key.Info.SubSchemaId!.Value];

				if (targetRef.Value.HasValue)
					deserializeStack.Push(new DeserializeState(refSchema, targetRef.Key, targetRef.Value.Value));
				else
				{
					target.ReadValues.Add(new(targetRef.Key, null));
				}

				continue;
			}

			if (deserializeStack.Count > 1)
			{
				deserializeStack.Pop();
				var parent = deserializeStack.Peek();

				parent.ReadValues.Add(new(target.element.Value, Create(target)));
			}
			else if (deserializeStack.Count == 1)
			{
				rootComponent = (T)Create(deserializeStack.Pop());
			}
		} while (deserializeStack.Count > 0);


		return rootComponent;
	}

	private object Create(DeserializeState target)
	{
		var i = Activator.CreateInstance(target.Schema.SchemaOf);

		var d = new Dictionary<DataSchemaElement, List<object?>>();

		foreach (var readValue in target.ReadValues)
		{
			if (readValue.Key.Info.Type == ESchemaElementType.ComplexType
			    && readValue.Key.Info.CollectionType is ECollectionType.Array)
			{
				if (!d.TryGetValue(readValue.Key, out var list))
				{
					list = new List<object?>();
					d[readValue.Key] = list;
				}

				list.Add(readValue.Value);
			}
			else
			{
				readValue.Key.FieldInfo.SetValue(i, readValue.Value);
			}
		}

		foreach (var k in d)
		{
			var array = Array.CreateInstance(k.Key.FieldInfo.FieldType.GetElementType(), k.Value.Count);

			for (int j = 0; j < k.Value.Count; j++)
			{
				array.SetValue(k.Value[j], j);
			}

			k.Key.FieldInfo.SetValue(i, array);
		}

		return i;
	}

	private async Task ReadComponentPart(IDbConnection dbConnection, DeserializeState state)
	{
		var elements = state.UnreadValues.ToSqlName();
		var tableName = state.Schema.SchemaOf.FullName;

		sqlBuilder.Clear();
		sqlBuilder.Append(
			$"SELECT {string.Join(',', elements)} FROM '{tableName}' ");
		sqlBuilder.Append($"WHERE Id = '{state.ComponentId}'");
		sqlBuilder.Append(';');

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		Console.WriteLine("SQL:\n" + cmd.CommandText);

		var reader = await cmd.ExecuteReaderAsync();

		if (!await reader.ReadAsync())
		{
			// TODO we can't find component with this id
			throw new Exception();
		}

		for (int i = 0; i < state.UnreadValues.Count; i++)
		{
			var element = state.UnreadValues[i];

			var elementType = element.Info.Type;
			if (element.Info.CollectionType is ECollectionType.Array)
			{
				var rawArray = reader.GetString(i);

				var obj = JsonNode.Parse(rawArray)!.AsArray();

				if (elementType == ESchemaElementType.ComplexType)
				{
					for (int j = 0; j < obj.Count; j++)
					{
						var r = obj[j]?.ToString();

						if (string.IsNullOrWhiteSpace(r))
						{
							state.Refs.Push(new(element, null));
						}
						else
						{
							state.Refs.Push(new(element, long.Parse(r)));
						}
					}

					continue;
				}

				var array = Array.CreateInstance(element.ToType(), obj.Count);

				for (int j = 0; j < obj.Count; j++)
				{
					array.SetValue(element.Info.Parse(obj[j]?.ToString()), j);
				}

				state.ReadValues.Add(new KeyValuePair<DataSchemaElement, object?>(element, array));
				continue;
			}

			if (elementType == ESchemaElementType.ComplexType)
			{
				state.Refs.Push(new(element, reader.GetInt64(i)));
				continue;
			}

			object? value = null;
			if (reader.IsDBNull(i))
			{
				value = null;
			}
			else
			{
				value = elementType.Read(reader, i);
			}

			state.ReadValues.Add(new KeyValuePair<DataSchemaElement, object?>(element, value));
		}

		state.UnreadValues.Clear();
	}

	private async Task<long> GetComponentIdAsync(IDbConnection dbConnection, Guid entityId, string componentName)
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

		Console.WriteLine("SQL:\n" + cmd.CommandText);

		return (long)await cmd.ExecuteScalarAsync();
	}

	public void Clear() { }
}