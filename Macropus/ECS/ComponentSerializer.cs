using System.Collections;
using System.Data;
using System.Text;
using Macropus.Database.Adapter;
using Macropus.ECS.Component;
using Macropus.Schema;

namespace Macropus.ECS;

public partial class ComponentSerializer : IDisposable
{
	private readonly IDbConnection dbConnection;

	public ComponentSerializer(IDbConnection dbConnection)
	{
		this.dbConnection = dbConnection;
	}

	public async Task SerializeAsync<T>(DataSchema schema, Guid entityId, T component) where T : struct, IComponent
	{
		using var transaction = dbConnection.BeginTransaction();
		try
		{
			var componentName = schema.SchemaOf.FullName;

			var stack = new Stack<SerializeState>();
			stack.Push(new SerializeState(schema, component));

			var componentId = 0;
			do
			{
				var target = stack.Peek();

				if (target.Value == null)
				{
					stack.Pop();
					if (target.TargetCollection != null)
						target.TargetCollection.Add(null);
					else
						ReturnInParent(stack, null);

					continue;
				}

				if (target.Unprocessed.Count > 0)
				{
					target.ProcessUnprocessed(stack);
					continue;
				}

				componentId = await InsertComponentPart(target);

				stack.Pop();

				if (target.TargetCollection != null)
				{
					target.TargetCollection.Add(componentId);
					continue;
				}

				ReturnInParent(stack, componentId);
			} while (stack.Count > 0);


			await AddEntityComponent(componentId, componentName!, entityId);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	private static void ReturnInParent(Stack<SerializeState> stack, object? componentId)
	{
		if (stack.Count > 0)
		{
			var parent = stack.Peek();

			parent.Unprocessed.RemoveAt(parent.Unprocessed.Count - 1);
			parent.Processed.Push(componentId);
		}
	}

	private async Task<int> InsertComponentPart(SerializeState data)
	{
		var fields = data.Schema.Elements.ToArray();
		if (fields.Length == 0)
			// TODO
			return -1;

		var tableName = data.Schema.SchemaOf.FullName;

		var sqlBuilder = new StringBuilder();
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

				sqlBuilder.Append("'{");

				foreach (var collectionValue in enumerable)
					sqlBuilder.Append(element.ToSqlInsert(collectionValue));

				sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
				sqlBuilder.Append("}', ");
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

	public Task<T?> DeserializeAsync<T>(DataSchema schema, Guid entityId)
		where T : struct, IComponent
	{
		throw new NotImplementedException();
	}

	private async Task AddEntityComponent(int componentId, string componentName, Guid entityId)
	{
		var sqlBuilder = new StringBuilder();
		sqlBuilder.Append(
			$"INSERT INTO '{ENTITIES_COMPONENTS_TABLE_NAME}' (ComponentId, ComponentName, EntityId) VALUES(");
		sqlBuilder.Append($"{componentId}, ");
		sqlBuilder.Append($"'{componentName}', ");
		sqlBuilder.Append($"\'{entityId.ToString("N")}\'");
		sqlBuilder.Append(");");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		await cmd.ExecuteNonQueryAsync();
	}

	public void Dispose()
	{
		dbConnection.Dispose();
	}
}