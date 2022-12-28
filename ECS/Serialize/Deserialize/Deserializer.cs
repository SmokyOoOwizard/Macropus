using System.Data;
using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.ECS.Component;
using Macropus.ECS.Serialize.Sql;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Deserialize;

class Deserializer : IClearable
{
	private static readonly Pool<ComponentDeserializeState> ComponentStatePool = new();

	private readonly Stack<ComponentDeserializeState> deserializeStack = new();
	private readonly SqlDeserializer deserializer = new();

	public async Task<T?> DeserializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId)
		where T : struct, IComponent
	{
		T rootComponent = default;

		var componentName = schema.SchemaOf.FullName;
		if (componentName == null)
			throw new Exception();

		var rootComponentId = await deserializer.GetComponentIdAsync(dbConnection, entityId, componentName);

		deserializeStack.Push(ComponentStatePool.Take().Init(schema, rootComponentId));

		do
		{
			var target = deserializeStack.Peek();

			var targetUnreadValues = target.GetUnreadFields();
			if (targetUnreadValues.Count > 0)
				await deserializer.ReadComponentPart(dbConnection, target);

			var targetRefNullable = target.TryGetRef();
			if (targetRefNullable != null)
			{
				ProcessRef(schema, targetRefNullable.Value, target);

				continue;
			}


			switch (deserializeStack.Count)
			{
				case > 1:
				{
					if (target.Element == null)
						// TODO
						throw new Exception();

					deserializeStack.Pop();
					var parent = deserializeStack.Peek();

					parent.AddReadRef(target.Element.Value, Create(target));

					target.Clear();
					break;
				}
				case 1:
					rootComponent = (T)Create(target);

					deserializeStack.Pop();
					target.Clear();
					break;
			}
		} while (deserializeStack.Count > 0);


		return rootComponent;
	}

	private void ProcessRef(
		DataSchema schema,
		KeyValuePair<DataSchemaElement, long?> targetRef,
		ComponentDeserializeState target
	)
	{
		var refSchema = schema.SubSchemas[targetRef.Key.Info.SubSchemaId!.Value];

		if (targetRef.Value.HasValue)
			deserializeStack.Push(ComponentStatePool.Take().Init(refSchema, targetRef.Key, targetRef.Value.Value));
		else
			target.AddReadRef(targetRef.Key, null);
	}

	private object Create(ComponentDeserializeState target)
	{
		var instance = Activator.CreateInstance(target.Schema.SchemaOf);

		foreach (var readValue in target.ReadValues)
			readValue.Key.FieldInfo.SetValue(instance, readValue.Value);

		foreach (var readRef in target.ReadRefs)
		{
			if (readRef.Key.Info.CollectionType is ECollectionType.Array)
			{
				CreateArray(readRef, instance);
			}
			else
				readRef.Key.FieldInfo.SetValue(instance, readRef.Value.First());
		}

		return instance!;
	}

	private static void CreateArray(KeyValuePair<DataSchemaElement, List<object?>> readRef, object? instance)
	{
		var elementType = readRef.Key.FieldInfo.FieldType.GetElementType();
		if (elementType == null)
			throw new Exception();

		var array = Array.CreateInstance(elementType, readRef.Value.Count);

		for (var i = 0; i < readRef.Value.Count; i++)
			array.SetValue(readRef.Value[i], i);

		readRef.Key.FieldInfo.SetValue(instance, array);
	}

	public void Clear()
	{
		foreach (var state in deserializeStack)
		{
			ComponentStatePool.Release(state);
		}

		deserializeStack.Clear();
		deserializer.Clear();
	}
}