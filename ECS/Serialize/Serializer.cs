using System.Collections;
using System.Data;
using Macropus.CoolStuff;
using Macropus.ECS.Component;
using Macropus.ECS.Serialize.Sql;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class Serializer : IClearable
{
	private readonly Stack<SerializeState> serializeStack = new();

	private readonly SqlSerializer serializer = new();

	public async Task SerializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId, T component)
		where T : struct, IComponent
	{
		using var transaction = dbConnection.BeginTransaction();
		try
		{
			var componentName = schema.SchemaOf.FullName;

			serializeStack.Push(new SerializeState(schema, component));

			var componentId = 0L;
			do
			{
				var target = serializeStack.Peek();

				var unprocessedNullable = target.TryGetUnprocessed();
				if (unprocessedNullable != null)
				{
					var unprocessed = unprocessedNullable.Value;
					var newTarget = unprocessed.Value.Dequeue();

					if (newTarget == null)
					{
						target.AddProcessed(unprocessed.Key, null);
						continue;
					}

					if (!unprocessed.Key.Info.SubSchemaId.HasValue)
						// TODO
						throw new Exception();

					var refSchema = schema.SubSchemas[unprocessed.Key.Info.SubSchemaId.Value];
					serializeStack.Push(new SerializeState(refSchema, newTarget, unprocessed.Key));
					continue;
				}

				serializeStack.Pop();

				componentId = await serializer.InsertComponent(dbConnection, target);
				if (target.ParentRef != null && serializeStack.Count > 0)
				{
					var parent = serializeStack.Peek();

					parent.AddProcessed(target.ParentRef.Value, componentId);
				}

				target.Clear();
			} while (serializeStack.Count > 0);


			await serializer.AddEntityComponent(dbConnection, componentId, componentName!, entityId);

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