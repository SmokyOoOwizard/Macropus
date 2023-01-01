using System.Data;
using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Database.Adapter;
using Macropus.ECS.Component;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Deserialize;

class Deserializer : IClearable
{
	private static readonly Pool<ComponentDeserializeState> ComponentStatePool = new();

	private readonly Stack<IDeserializeState> deserializeStack = new();

	public async Task<T?> DeserializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId)
		where T : struct, IComponent
	{
		T rootComponent = default;

		var componentName = schema.SchemaOf.FullName;
		if (componentName == null)
			throw new Exception();

		var componentIdCmd = DbCommandCache.GetWWCmd(dbConnection, entityId, componentName);
		using var componentIdReader = await componentIdCmd.ExecuteReaderAsync();
		await componentIdReader.ReadAsync();
		var rootComponentId = componentIdReader.GetInt64(0);

		deserializeStack.Push(ComponentStatePool.Take().Init(schema, rootComponentId));

		do
		{
			var target = deserializeStack.Peek();
			await target.Read(dbConnection);
			if (target.HasRefs())
			{
				deserializeStack.Push(target.PopSomeRefs());
				continue;
			}

			deserializeStack.Pop();
			
			switch (target)
			{
				case ITargetDeserializeState tds:
				{
					var parent = deserializeStack.Peek();

					var obj = tds.Create();

					parent.AddRef(tds.Target, obj);
					break;
				}
				case ComponentDeserializeState cds:
				{
					if (deserializeStack.Count > 1)
						throw new Exception();

					rootComponent = (T)(cds.Create() ?? default(T));

					target.Clear();
					break;
				}
			}
		} while (deserializeStack.Count > 0);


		DbCommandCache.Clear(dbConnection);

		return rootComponent;
	}

	public void Clear()
	{
		foreach (var state in deserializeStack)
		{
			//ComponentStatePool.Release(state);
		}

		deserializeStack.Clear();
	}
}