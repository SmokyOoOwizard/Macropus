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
					case ParallelSerializeState pss:
						await ProcessParallelSS(dbConnection, pss).ConfigureAwait(false);
						break;
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
	private async Task ProcessParallelSS(IDbConnection dbConnection, ParallelSerializeState pss)
	{
		var r = await serializer.InsertComponent(dbConnection, pss, 50).ConfigureAwait(false);
		serializeStack.Pop();
		if (pss.ParentRef != null && serializeStack.Count > 0)
		{
			var parent = serializeStack.Peek();
			// ReSharper disable once InconsistentNaming
			if (parent is not ComponentSerializeState parentCSS)
				throw new Exception();

			parentCSS.AddRangeProcessed(pss.ParentRef.Value, r);
		}

		StatePool.Release(pss);
	}

	// ReSharper disable once InconsistentNaming
	private async Task<long?> ProcessComponentSS(
		IDbConnection dbConnection,
		DataSchema schema,
		ComponentSerializeState css
	)
	{
		var unprocessedNullable = css.TryGetUnprocessed();
		if (unprocessedNullable != null)
		{
			ProcessUnprocessedComponentSS(schema, css, unprocessedNullable.Value);
			return null;
		}

		serializeStack.Pop();

		var componentId = await serializer.InsertComponent(dbConnection, css);
		if (css.ParentRef != null && serializeStack.Count > 0)
		{
			var parent = serializeStack.Peek();
			// ReSharper disable once InconsistentNaming
			if (parent is not ComponentSerializeState parentCSS)
				throw new Exception();

			parentCSS.AddProcessed(css.ParentRef.Value, componentId);
		}

		StatePool.Release(css);

		return componentId;
	}

	// ReSharper disable once InconsistentNaming
	private void ProcessUnprocessedComponentSS(
		DataSchema schema,
		ComponentSerializeState css,
		KeyValuePair<DataSchemaElement, Queue<object?>> unprocessed
	)
	{
		if (!unprocessed.Key.Info.SubSchemaId.HasValue)
			// TODO
			throw new Exception();

		var refSchema = schema.SubSchemas[unprocessed.Key.Info.SubSchemaId.Value];
		if (refSchema.SubSchemas.Count == 0)
		{
			serializeStack.Push(StatePool.ParallelSerializeStatePool.Take()
				.Init(refSchema, unprocessed.Value, unprocessed.Key));
			return;
		}

		var newTarget = unprocessed.Value.Dequeue();
		if (newTarget == null)
		{
			css.AddProcessed(unprocessed.Key, null);
			return;
		}

		serializeStack.Push(StatePool.ComponentSerializeStatePool.Take().Init(refSchema, newTarget, unprocessed.Key));
	}

	public void Clear()
	{
		foreach (var state in serializeStack)
			StatePool.Release(state);

		serializeStack.Clear();
		serializer.Clear();
	}
}