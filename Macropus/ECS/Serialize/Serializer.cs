using System.Collections;
using System.Data;
using System.Text;
using Macropus.CoolStuff;
using Macropus.Database.Adapter;
using Macropus.ECS.Component;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class Serializer : IClearable
{
	private readonly Stack<SerializeState> serializeQueue = new();
	private readonly StringBuilder sqlBuilder = new();

	public async Task SerializeAsync<T>(IDbConnection dbConnection, DataSchema schema, Guid entityId, T component)
		where T : struct, IComponent
	{
		using var transaction = dbConnection.BeginTransaction();
		try
		{
			var componentName = schema.SchemaOf.FullName;

			serializeQueue.Push(new SerializeState(schema, component));

			var componentId = 0;
			do
			{
				var target = serializeQueue.Peek();

				if (target.Value == null)
				{
					NullValue(target);
					continue;
				}

				if (target.Unprocessed.Count > 0)
				{
					target.ProcessUnprocessed(serializeQueue);
					continue;
				}

				try
				{
					componentId = await InsertComponentPart(dbConnection, target);
				}
				finally
				{
					serializeQueue.Pop();
					target.Clear();
				}


				if (target.TargetCollection != null)
				{
					target.TargetCollection.Add(componentId);
					continue;
				}

				ReturnInParent(componentId);
			} while (serializeQueue.Count > 0);


			await AddEntityComponent(dbConnection, componentId, componentName!, entityId);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	private void NullValue(SerializeState target)
	{
		serializeQueue.Pop();
		if (target.TargetCollection != null)
			target.TargetCollection.Add(null);
		else
			ReturnInParent(null);

		target.Clear();
	}

	private void ReturnInParent(object? componentId)
	{
		if (serializeQueue.Count > 0)
		{
			var parent = serializeQueue.Peek();

			parent.Unprocessed.RemoveAt(parent.Unprocessed.Count - 1);
			parent.Processed.Push(componentId);
		}
	}

	private async Task<int> InsertComponentPart(IDbConnection dbConnection, SerializeState data)
	{
		var fields = data.Schema.Elements.ToArray();
		if (fields.Length == 0)
			// TODO
			return -1;

		var tableName = data.Schema.SchemaOf.FullName;

		sqlBuilder.Clear();
		sqlBuilder.Append($"INSERT INTO '{tableName}' (");
		sqlBuilder.Append(string.Join(',', fields.Select(e => e.Info.ToSqlName())));
		sqlBuilder.Append(") VALUES (");

		foreach (var element in fields)
		{
			var value = element.FieldInfo.GetValue(data.Value);
			if (element.Info.CollectionType is ECollectionType.Array)
			{
				IEnumerable enumerable;
				if (element.Info.Type is ESchemaElementType.ComplexType)
				{
					var ids = data.Processed.Pop();
					enumerable = (ids as IEnumerable)!;
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
				sqlBuilder.Append(element.ToSqlInsert(data.Processed.Pop()));
			else
				sqlBuilder.Append(element.ToSqlInsert(value));
		}

		sqlBuilder.Remove(sqlBuilder.Length - 2, 2);

		sqlBuilder.Append("); select last_insert_rowid();");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		Console.WriteLine("SQL:\n" + cmd.CommandText);

		var reader = await cmd.ExecuteReaderAsync();

		await reader.ReadAsync();

		return reader.GetInt32(0);
	}

	private async Task AddEntityComponent(
		IDbConnection dbConnection,
		int componentId,
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
		serializeQueue.Clear();
		sqlBuilder.Clear();
	}
}