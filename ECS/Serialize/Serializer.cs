using System.Data;
using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.ECS.Component;
using Macropus.ECS.Serialize.Sql;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class Serializer : IClearable
{
	private readonly Stack<ISerializeState> serializeStack = new();
	private readonly SqlSerializer serializer = new();

	private readonly Pool<ComponentSerializeState> componentStatePool = new();
	private readonly Pool<ParallelSerializeState> parallelStatePool = new();

	public async Task SerializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId, T component)
		where T : struct, IComponent
	{
		using var transaction = dbConnection.BeginTransaction();
		try
		{
			var componentName = schema.SchemaOf.FullName;

			serializeStack.Push(componentStatePool.Take().Init(schema, component));

			var componentId = 0L;
			do
			{
				var target = serializeStack.Peek();
				if (target is ComponentSerializeState css)
				{
					var unprocessedNullable = css.TryGetUnprocessed();
					if (unprocessedNullable != null)
					{
						var unprocessed = unprocessedNullable.Value;
						if (!unprocessed.Key.Info.SubSchemaId.HasValue)
							// TODO
							throw new Exception();

						var refSchema = schema.SubSchemas[unprocessed.Key.Info.SubSchemaId.Value];
						if (refSchema.SubSchemas.Count == 0)
						{
							serializeStack.Push(parallelStatePool.Take()
								.Init(refSchema, unprocessed.Value, unprocessed.Key));
							continue;
						}

						var newTarget = unprocessed.Value.Dequeue();
						if (newTarget == null)
						{
							css.AddProcessed(unprocessed.Key, null);
							continue;
						}

						serializeStack.Push(componentStatePool.Take().Init(refSchema, newTarget, unprocessed.Key));
						continue;
					}

					serializeStack.Pop();

					componentId = await serializer.InsertComponent(dbConnection, css);
					if (css.ParentRef != null && serializeStack.Count > 0)
					{
						var parent = serializeStack.Peek();
						if (parent is not ComponentSerializeState parentCSS)
							throw new Exception();

						parentCSS.AddProcessed(css.ParentRef.Value, componentId);
					}

					componentStatePool.Release(target);
				}
				else if (target is ParallelSerializeState pss)
				{
					var r = await serializer.InsertComponent(dbConnection, pss, 50).ConfigureAwait(false);
						serializeStack.Pop();
					if (pss.ParentRef != null && serializeStack.Count > 0)
					{
						var parent = serializeStack.Peek();
						if (parent is not ComponentSerializeState parentCSS)
							throw new Exception();

						parentCSS.AddRangeProcessed(pss.ParentRef.Value, r);
					}
					
					parallelStatePool.Release(pss);
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
			serializer.Clear();
		}
	}

	public void Clear()
	{
		serializeStack.Clear();
		serializer.Clear();
	}
}