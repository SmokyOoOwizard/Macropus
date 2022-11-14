using System.Data;
using System.Text;
using System.Text.Json.Nodes;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Macropus.ECS.Component;
using Macropus.ECS.Extensions;
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

			var targetUnreadValues = target.GetUnreadFields();
			if (targetUnreadValues.Count > 0)
				await ReadComponentPart(dbConnection, target);

			var targetRefNullable = target.TryGetRef();
			if (targetRefNullable != null)
			{
				var targetRef = targetRefNullable.Value;
				var refSchema = schema.SubSchemas[targetRef.Key.Info.SubSchemaId!.Value];

				if (targetRef.Value.HasValue)
					deserializeStack.Push(new DeserializeState(refSchema, targetRef.Key, targetRef.Value.Value));
				else
				{
					target.AddReadRef(targetRef.Key, null);
				}

				continue;
			}

			if (deserializeStack.Count > 1)
			{
				deserializeStack.Pop();
				var parent = deserializeStack.Peek();

				parent.AddReadRef(target.Element.Value, Create(target));

				target.Clear();
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
		var instance = Activator.CreateInstance(target.Schema.SchemaOf);

		foreach (var readValue in target.ReadValues)
		{
			readValue.Key.FieldInfo.SetValue(instance, readValue.Value);
		}

		foreach (var readRef in target.ReadRefs)
		{
			if (readRef.Key.Info.CollectionType is ECollectionType.Array)
			{
				var array = Array.CreateInstance(readRef.Key.FieldInfo.FieldType.GetElementType(), readRef.Value.Count);

				for (int j = 0; j < readRef.Value.Count; j++)
				{
					array.SetValue(readRef.Value[j], j);
				}

				readRef.Key.FieldInfo.SetValue(instance, array);
			}
			else
			{
				readRef.Key.FieldInfo.SetValue(instance, readRef.Value.First());
			}
		}

		return instance!;
	}

	private async Task ReadComponentPart(IDbConnection dbConnection, DeserializeState state)
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
				var rawArray = reader.GetString(i);

				var obj = JsonNode.Parse(rawArray)!.AsArray();

				if (elementType == ESchemaElementType.ComplexType)
				{
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

				state.ReadValues.Add(new KeyValuePair<DataSchemaElement, object?>(element, array));
				continue;
			}

			if (elementType == ESchemaElementType.ComplexType)
			{
				state.AddRef(element, reader.GetInt64(i));
				continue;
			}

			object? value = reader.IsDBNull(i) ? null : elementType.Read(reader, i);
			state.ReadValues.Add(new KeyValuePair<DataSchemaElement, object?>(element, value));
		}

		state.MarkUnreadFieldsAsRead();
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

		return (long)await cmd.ExecuteScalarAsync();
	}

	public void Clear()
	{
		deserializeStack.Clear();
		sqlBuilder.Clear();
	}
}