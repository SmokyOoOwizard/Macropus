using System.Data;
using ECS.Schema;
using ECS.Serialize.Serialize.State;
using ECS.Serialize.Serialize.State.Impl;
using ECS.Serialize.Sql;
using Macropus.CoolStuff;
using Macropus.ECS.Component;

namespace ECS.Serialize.Serialize;

class Serializer : IClearable
{
	private static readonly StatePool StatePool = StatePool.Instance;

	private readonly Stack<ISerializeState> serializeStack = new();
	private readonly SqlSerializer serializer = new();


	public async Task SerializeAsync(IDbConnection dbConnection, DataSchema schema, Guid entityId, IComponent component)
	{
		using var transaction = dbConnection.BeginTransaction();
		try
		{
			var componentName = schema.SchemaOf.FullName;

			serializeStack.Push(StatePool.ComponentSerializeStatePool.Take().Init(schema, component));

			var componentId = 0L;
			do
			{
				var target = serializeStack.Peek();
				switch (target)
				{
					case ComponentSerializeState css:
					{
						var tmp = await ProcessComponentSS(dbConnection, schema, css).ConfigureAwait(false);
						if (tmp == null)
							continue;

						componentId = tmp.Value;
						break;
					}
				}
			} while (serializeStack.Count > 0);


			await serializer.AddEntityComponent(dbConnection, componentId, componentName!, entityId)
				.ConfigureAwait(false);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
		finally
		{
			DbCommandCache.Clear(dbConnection);
		}
	}

	// ReSharper disable once InconsistentNaming
	private async Task<long?> ProcessComponentSS(
		IDbConnection dbConnection,
		DataSchema schema,
		ComponentSerializeState css
	)
	{
		serializeStack.Pop();

		var componentId = await serializer.InsertComponent(dbConnection, css);

		StatePool.Release(css);

		return componentId;
	}

	public void Clear()
	{
		foreach (var state in serializeStack)
			StatePool.Release(state);

		serializeStack.Clear();
		serializer.Clear();
	}
}