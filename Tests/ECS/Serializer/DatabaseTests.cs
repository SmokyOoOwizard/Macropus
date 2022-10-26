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
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);
	}

	[Fact]
	public async Task TryCreateTableByDataSchemaWithAlreadyExistsSameTableTest()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);

		await serializer.CreateTablesBySchema(schema);
		await serializer.CreateTablesBySchema(schema);
	}
}