using Macropus.ECS;
using Macropus.Schema;
using Tests.Schema;
using Tests.Utils.Random;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.ECS.Serializer;

public class SerializeTests : TestsWithDatabase
{
	public SerializeTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async Task SerializeComponentTest()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);

		var entityId = Guid.NewGuid();
		var testComponent = RandomUtils.GetRandomDataSchemaTestTypeComponent();

		await serializer.SerializeAsync(schema, entityId, testComponent);

		var deserializedComponent =
			await serializer.DeserializeAsync<DataSchemaTestTypeComponent>(schema, entityId);

		Assert.Equal(testComponent, deserializedComponent);
	}
}