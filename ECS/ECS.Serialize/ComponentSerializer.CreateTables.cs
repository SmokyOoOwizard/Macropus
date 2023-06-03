using System.Text;
using ECS.Schema;
using ECS.Serialize.Extensions;
using ECS.Serialize.Models;
using LinqToDB;
using Macropus.Database.Extensions;

namespace ECS.Serialize;

public partial class ComponentSerializer
{
	public async Task CreateTablesBySchema(DataSchema schema)
	{
		await using var transaction = await dataConnection.BeginTransactionAsync();
		try
		{
			if (!await dataConnection.TableAlreadyExists(EntitiesComponentsTable.TABLE_NAME))
				await CreateEntitiesComponentsTable();

			await CreateTableBySchema(schema);

			await transaction.CommitAsync();
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}

	private async Task CreateEntitiesComponentsTable()
	{
		await dataConnection.CreateTableAsync<EntitiesComponentsTable>(EntitiesComponentsTable.TABLE_NAME);
	}

	private async Task CreateTableBySchema(DataSchema schema)
	{
		var simpleFields = schema.Elements;

		var tableName = ComponentFormatUtils.NormalizeName(schema.SchemaOf.FullName);

		if (string.IsNullOrWhiteSpace(tableName))
			throw new ArgumentNullException(nameof(schema.SchemaOf));

		if (await dataConnection.TableAlreadyExists(tableName))
			// TODO check table
			return;

		var sqlBuilder = new StringBuilder();
		sqlBuilder.Append($"CREATE TABLE '{tableName}' (");
		sqlBuilder.Append("Id INTEGER PRIMARY KEY");
		if (simpleFields.Any())
		{
			sqlBuilder.Append(", ");
			sqlBuilder.Append(string.Join(',', simpleFields.ToSql()));
		}

		sqlBuilder.Append(");");

		var cmd = dataConnection.CreateCommand();
		cmd.CommandText = sqlBuilder.ToString();

		await cmd.ExecuteNonQueryAsync();
	}
}