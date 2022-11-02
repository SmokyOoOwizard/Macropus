using System.Text;
using Macropus.Database.Adapter;
using Macropus.Database.Extensions;
using Macropus.Schema;

namespace Macropus.ECS;

public partial class ComponentSerializer
{
	private const string ENTITIES_COMPONENTS_TABLE_NAME = "EntitesComponents";

	public async Task CreateTablesBySchema(DataSchema schema)
	{
		var subSchemas = schema.SubSchemas.Select(kv => kv.Value).Where(s => s != schema);

		using var transaction = dbConnection.BeginTransaction();
		try
		{
			if (!await dbConnection.TableAlreadyExists(ENTITIES_COMPONENTS_TABLE_NAME))
				await CreateEntitiesComponentsTable();


			await CreateTableBySchema(schema);
			foreach (var subSchema in subSchemas)
				await CreateTableBySchema(subSchema);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	private async Task CreateEntitiesComponentsTable()
	{
		var sqlBuilder = new StringBuilder();
		sqlBuilder.Append($"CREATE TABLE '{ENTITIES_COMPONENTS_TABLE_NAME}' (");
		sqlBuilder.Append("Id INTEGER PRIMARY KEY, ");
		sqlBuilder.Append("ComponentId INTEGER NOT NULL, ");
		sqlBuilder.Append("ComponentName TEXT NOT NULL, ");
		sqlBuilder.Append("EntityId TEXT NOT NULL COLLATE NOCASE");
		sqlBuilder.Append(");");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		await cmd.ExecuteNonQueryAsync();
	}

	private async Task CreateTableBySchema(DataSchema schema)
	{
		var tableName = schema.SchemaOf.FullName;

		if (string.IsNullOrWhiteSpace(tableName))
			throw new ArgumentNullException(nameof(schema.SchemaOf));

		if (await dbConnection.TableAlreadyExists(tableName))
			// TODO check table
			return;

		var simpleFields = schema.Elements;
		if (!simpleFields.Any())
			// TODO schema must have fields
			throw new Exception();

		var sqlBuilder = new StringBuilder();
		sqlBuilder.Append($"CREATE TABLE '{tableName}' (");
		sqlBuilder.Append("Id INTEGER PRIMARY KEY, ");
		sqlBuilder.Append(string.Join(',', simpleFields.ToSql()));
		sqlBuilder.Append(");");

		var cmd = dbConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		await cmd.ExecuteNonQueryAsync();
	}
}