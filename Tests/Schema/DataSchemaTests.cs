using Macropus.Schema;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Schema;

public class DataSchemaTests : TestsWrapper
{
	public DataSchemaTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void CreateSchemaTest()
	{
		var schema = DataSchemaUtils.CreateSchema<DataSchemaTestTypeComponent>();

		Assert.NotNull(schema);

		Assert.Equal(schema.SubSchemas.Values.Count(), schema.SubSchemas.Values.Distinct().Count());
	}

	[Fact]
	public void SchemaCorrectTypeTest()
	{
		var schema = DataSchemaUtils.CreateSchema<DataSchemaTestTypeComponent>();
		Assert.NotNull(schema);

		Assert.True(schema.IsCorrectType<DataSchemaTestTypeComponent>());
		Assert.False(schema.IsCorrectType<DataSchemaSubSchemaComponent2>());
	}
}