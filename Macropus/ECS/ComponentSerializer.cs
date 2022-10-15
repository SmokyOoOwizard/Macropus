using System.Data;
using System.Text;
using Macropus.Database.Adapter;
using Macropus.Database.Extensions;
using Macropus.ECS.Component;
using Macropus.Schema;

namespace Macropus.ECS;

public class ComponentSerializer : IDisposable
{
	private readonly IDataSchemasStorage componentSchemas;
	private readonly IDbConnection dbConnection;

	public ComponentSerializer(IDbConnection dbConnection, IDataSchemasStorage componentSchemas)
	{
		this.dbConnection = dbConnection;
		this.componentSchemas = componentSchemas;
	}

	public async Task<bool> SerializeAsync<T>(
		IDbConnection connection,
		IDictionary<Guid, DataSchema> subSchemas,
		T component
	)
		where T : struct, IComponent
	{
		throw new NotImplementedException();
	}

	public Task<T?> DeserializeAsync<T>(IDbConnection connection, IDictionary<Guid, DataSchema> subSchemas)
		where T : struct, IComponent
	{
		throw new NotImplementedException();
	}

	public async Task CreateTablesBySchema(Guid schemaId)
	{
		var mainSchema = componentSchemas.GetSchema(schemaId);

		var subSchemas = mainSchema.GetSubSchemas(componentSchemas);

		using var transaction = dbConnection.BeginTransaction();
		try
		{
			await CreateTableBySchema(mainSchema);
			foreach (var subSchema in subSchemas)
				await CreateTableBySchema(subSchema);

			transaction.Commit();
		}
		catch (Exception e)
		{
			transaction.Rollback();
			throw;
		}
	}

	private async Task CreateTableBySchema(DataSchema schema)
	{
		if (string.IsNullOrWhiteSpace(schema.Name))
			throw new ArgumentNullException(nameof(schema.Name));

		var tableName = schema.Name;
		if (await dbConnection.TableAlreadyExists(tableName))
			// TODO check table
			return;

		var simpleFields = schema.Elements;
		if (!simpleFields.Any())
		{
			return;
			// TODO schema must have fields
			throw new Exception();
		}

		var sqlBuilder = new StringBuilder();
		sqlBuilder.Append($"CREATE TABLE '{tableName}' (");
		sqlBuilder.Append("Id TEXT NOT NULL COLLATE NOCASE, ");
		sqlBuilder.Append(string.Join(',', simpleFields.ToSql()));
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