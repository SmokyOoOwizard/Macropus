using System.Collections;
using System.Data;
using System.Text;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Macropus.ECS.Component;
using Macropus.ECS.Serialize.Extensions;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class Serializer : IClearable
{
	private readonly Stack<SerializeState> serializeStack = new();
	private readonly StringBuilder sqlBuilder = new();

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

				componentId = await InsertComponent(dbConnection, target);
				if (target.ParentRef != null && serializeStack.Count > 0)
				{
					var parent = serializeStack.Peek();

					parent.AddProcessed(target.ParentRef.Value, componentId);
				}

				target.Clear();
			} while (serializeStack.Count > 0);


			await AddEntityComponent(dbConnection, componentId, componentName!, entityId);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	private async Task<int> InsertComponent(IDbConnection dbConnection, SerializeState target)
	{
		var fields = target.Schema.Elements;

		var tableName = target.Schema.SchemaOf.FullName;

		sqlBuilder.Clear();
		sqlBuilder.Append($"INSERT INTO '{tableName}' (");
		sqlBuilder.Append(string.Join(',', fields.Select(e => e.Info.ToSqlName())));
		sqlBuilder.Append(") VALUES (");

		foreach (var element in fields)
		{
			var value = element.FieldInfo.GetValue(target.Value);
			if (element.Info.CollectionType is ECollectionType.Array)
			{
				IEnumerable enumerable;
				if (element.Info.Type is ESchemaElementType.ComplexType)
				{
					var ids = target.GetProcessed(element);
					ids.Reverse();
					enumerable = ids;
				}
				else
				{
					enumerable = (value as IEnumerable)!;
				}

				if (enumerable == null)
					// TODO
					throw new Exception();

				sqlBuilder.Append("json('[");

				foreach (var collectionValue in enumerable)
					sqlBuilder.Append(element.ToSqlInsert(collectionValue).Replace('\'', '"'));

				sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
				sqlBuilder.Append("]'), ");
				continue;
			}

			if (element.Info.Type is ESchemaElementType.ComplexType)
				sqlBuilder.Append(element.ToSqlInsert(target.GetProcessed(element).First()));
			else
				sqlBuilder.Append(element.ToSqlInsert(value));
		}

		sqlBuilder.Remove(sqlBuilder.Length - 2, 2);

		sqlBuilder.Append("); select last_insert_rowid();");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		var reader = await cmd.ExecuteReaderAsync();

		await reader.ReadAsync();

		return reader.GetInt32(0);
	}

	private async Task AddEntityComponent(
		IDbConnection dbConnection,
		long componentId,
		string componentName,
		Guid entityId
	)
	{
		sqlBuilder.Clear();
		sqlBuilder.Append(
			$"INSERT INTO '{ComponentSerializer.ENTITIES_COMPONENTS_TABLE_NAME}' (ComponentId, ComponentName, EntityId) VALUES(");
		sqlBuilder.Append($"{componentId}, ");
		sqlBuilder.Append($"'{componentName}', ");
		sqlBuilder.Append($"\'{entityId.ToString("N")}\'");
		sqlBuilder.Append(");");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		await cmd.ExecuteNonQueryAsync();
	}


	public void Clear()
	{
		serializeStack.Clear();
		sqlBuilder.Clear();
	}
}