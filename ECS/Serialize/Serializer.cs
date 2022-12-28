using System.Data;
using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.ECS.Component;
using Macropus.ECS.Serialize.Sql;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class Serializer : IClearable
{
	private static readonly Pool<ComponentSerializeState> ComponentStatePool = new();
	private static readonly Pool<ParallelSerializeState> ParallelStatePool = new();
	
	private readonly Stack<ISerializeState> serializeStack = new();
	private readonly SqlSerializer serializer = new();


	public async Task SerializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId, T component)
		where T : struct, IComponent
	{
		using var transaction = dbConnection.BeginTransaction();
		try
		{
			var componentName = schema.SchemaOf.FullName;

			serializeStack.Push(ComponentStatePool.Take().Init(schema, component));

			var componentId = 0L;
			do
			{
				var target = serializeStack.Peek();
				switch (target)
				{
					case ComponentSerializeState css:
					{
						var tmp = await ProcessComponentSS(dbConnection, schema, css, target).ConfigureAwait(false);
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
			Clear();
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

		ParallelStatePool.Release(pss);
	}

	// ReSharper disable once InconsistentNaming
	private async Task<long?> ProcessComponentSS(
		IDbConnection dbConnection,
		DataSchema schema,
		ComponentSerializeState css,
		ISerializeState target
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

		ComponentStatePool.Release(target);

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
			serializeStack.Push(ParallelStatePool.Take()
				.Init(refSchema, unprocessed.Value, unprocessed.Key));
			return;
		}

		var newTarget = unprocessed.Value.Dequeue();
		if (newTarget == null)
		{
			css.AddProcessed(unprocessed.Key, null);
			return;
		}

		serializeStack.Push(ComponentStatePool.Take().Init(refSchema, newTarget, unprocessed.Key));
	}

	public void Clear()
	{
		foreach (var state in serializeStack)
		{
			state.Clear();
			switch (state)
			{
				case ComponentSerializeState css:
					ComponentStatePool.Release(css);
					break;
				case ParallelSerializeState pss:
					ParallelStatePool.Release(pss);
					break;
			}
		}

		serializeStack.Clear();
		serializer.Clear();
	}
}