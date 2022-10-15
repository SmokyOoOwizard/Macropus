using Macropus.Schema;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Schema;

public class DataSchemaTests : TestsWrapper
{
	public DataSchemaTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void CreateDataSchema()
	{
		var subSchemas = new List<DataSchema>();
		var schema = DataSchemaUtils.CreateSchema<DataSchemaTestTypeComponent>(subSchemas);

		Assert.NotNull(schema);
		Assert.NotEmpty(subSchemas);
	}
}