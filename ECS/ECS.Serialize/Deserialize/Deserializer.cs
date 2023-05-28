using System.Data;
using ECS.Schema;
using ECS.Serialize.Deserialize.State;
using ECS.Serialize.Deserialize.State.Impl;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Macropus.ECS.Component;

namespace ECS.Serialize.Deserialize;

class Deserializer : IClearable
{
	private static readonly StatePool StatePool = StatePool.Instance;

	private readonly Stack<IDeserializeState> deserializeStack = new();

	public async Task<T?> DeserializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId)
		where T : struct, IComponent
	{
		T rootComponent = default;

		var componentName = schema.SchemaOf.FullName;
		if (componentName == null)
			throw new Exception();

		long rootComponentId;
		var componentIdCmd = DbCommandCache.GetComponentIdCmd(dbConnection, entityId, componentName);
		using (var componentIdReader = await componentIdCmd.ExecuteReaderAsync())
		{
			await componentIdReader.ReadAsync();
			rootComponentId = componentIdReader.GetInt64(0);
		}

		deserializeStack.Push(StatePool.DeserializeStatePool.Take().Init(schema, rootComponentId));

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

			StatePool.Release(target);
		} while (deserializeStack.Count > 0);


		DbCommandCache.Clear(dbConnection);

		return rootComponent;
	}

	public void Clear()
	{
		foreach (var state in deserializeStack)
		{
			StatePool.Release(state);
		}

		deserializeStack.Clear();
	}
}