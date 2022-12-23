using System.Data;
using Macropus.CoolStuff;
using Macropus.ECS.Component;
using Macropus.ECS.Serialize.Sql;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class Deserializer : IClearable
{
	private readonly Stack<DeserializeState> deserializeStack = new();
	private readonly SqlDeserializer deserializer = new();

	public async Task<T?> DeserializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId)
		where T : struct, IComponent
	{
		var componentName = schema.SchemaOf.FullName;

		var rootComponentId = await deserializer.GetComponentIdAsync(dbConnection, entityId, componentName!);
		T rootComponent = default;

		deserializeStack.Push(new DeserializeState(schema, rootComponentId));

		do
		{
			var target = deserializeStack.Peek();

			var targetUnreadValues = target.GetUnreadFields();
			if (targetUnreadValues.Count > 0)
				await deserializer.ReadComponentPart(dbConnection, target);

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

	public void Clear()
	{
		deserializeStack.Clear();
		deserializer.Clear();
	}
}