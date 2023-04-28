using Macropus.Schema;
using Tests.Utils;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Schema;

public class DataSchemaTests : TestsWrapper
{
	public DataSchemaTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void CreateSchemaTest()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		Assert.NotNull(schema);

		Assert.Equal(schema.SubSchemas.Values.Count(), schema.SubSchemas.Values.Distinct().Count());
	}
}