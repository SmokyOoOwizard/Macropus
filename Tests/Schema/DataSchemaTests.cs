using Macropus.Schema;
using Macropus.Schema.Impl;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Schema;

public class DataSchemaTests : TestsWrapper
{
	public DataSchemaTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void CreateSchemaTest()
	{
		var schemasStorage = new DataSchemasMemoryStorage();
		var schema = DataSchemaUtils.CreateSchema<DataSchemaTestTypeComponent>(schemasStorage);

		Assert.NotNull(schema);
		Assert.NotEmpty(schemasStorage);
	}

	[Fact]
	public void SchemaFullCorrectTypeTest()
	{
		var schemasStorage = new DataSchemasMemoryStorage();
		var schema = DataSchemaUtils.CreateSchema<DataSchemaTestTypeComponent>(schemasStorage);
		Assert.NotNull(schema);

		Assert.True(schema.IsFullCorrectType<DataSchemaTestTypeComponent>(schemasStorage));
		Assert.False(schema.IsFullCorrectType<DataSchemaSubSchemaComponent2>(schemasStorage));
	}

	[Fact]
	public void SchemaCorrectTypeTest()
	{
		var schemasStorage = new DataSchemasMemoryStorage();
		var schema = DataSchemaUtils.CreateSchema<DataSchemaTestTypeComponent>(schemasStorage);
		Assert.NotNull(schema);

		Assert.True(schema.IsCorrectType<DataSchemaTestTypeComponent>());
		Assert.False(schema.IsCorrectType<DataSchemaSubSchemaComponent2>());
	}
}