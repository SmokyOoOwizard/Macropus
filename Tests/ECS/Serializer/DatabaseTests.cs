using Macropus.ECS;
using Macropus.Schema;
using Tests.Schema;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.ECS.Serializer;

public class DatabaseTests : TestsWithDatabase
{
	public DatabaseTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async Task CreateTableByDataSchemaTest()
	{
		var schemasStorage = new DataSchemasMemoryStorage();

		var subSchemas = new List<DataSchema>();
		var schema = DataSchemaUtils.CreateSchema<DataSchemaTestTypeComponent>(subSchemas);

		schemasStorage.AddSchema(schema);
		foreach (var subSchema in subSchemas)
			schemasStorage.AddSchema(subSchema);

		using var serializer = new ComponentSerializer(DbConnection, schemasStorage);

		await serializer.CreateTablesBySchema(schema.Id);
	}

	[Fact]
	public async Task TryCreateTableByDataSchemaWithAlreadyExistsSameTableTest()
	{
		var schemasStorage = new DataSchemasMemoryStorage();

		var subSchemas = new List<DataSchema>();
		var schema = DataSchemaUtils.CreateSchema<DataSchemaTestTypeComponent>(subSchemas);

		schemasStorage.AddSchema(schema);
		foreach (var subSchema in subSchemas)
			schemasStorage.AddSchema(subSchema);

		using var serializer = new ComponentSerializer(DbConnection, schemasStorage);

		await serializer.CreateTablesBySchema(schema.Id);
		await serializer.CreateTablesBySchema(schema.Id);
	}
}