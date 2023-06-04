using ECS.Schema;
using ECS.Schema.Extensions;
using ECS.Tests.Utils.Schema;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Tests.Schema;

public class ColdDataSchemaTests : TestsWrapper
{
	public ColdDataSchemaTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void PackSchemaTest()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();
		var coldSchema = schema.Pack();

		Assert.NotNull(coldSchema);


		DataSchemaElementUtils.ElementsEqualsColdElements(schema.Elements, coldSchema.Elements);

		Assert.Equal(schema.SubSchemas.Count, coldSchema.SubSchemas.Count);
		foreach (var subSchemaPair in schema.SubSchemas)
		{
			var subSchema = subSchemaPair.Value;
			coldSchema.SubSchemas.TryGetValue(subSchemaPair.Key, out var coldSubSchema);

			Assert.NotNull(coldSubSchema);
			DataSchemaElementUtils.ElementsEqualsColdElements(subSchema.Elements, coldSubSchema);
		}
	}

	[Fact]
	public void UnpackSchemaTest()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();
		var coldSchema = schema.Pack();

		var unpackedSchema = coldSchema.Unpack<DataSchemaTestTypeComponent>();
		Assert.NotNull(unpackedSchema);

		schema.AssertEquals(unpackedSchema);
	}

	[Fact]
	public void SchemaCorrectTypeTest()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();
		var coldSchema = schema.Pack();

		Assert.NotNull(schema);

		Assert.True(coldSchema.IsCorrectType<DataSchemaTestTypeComponent>());
		Assert.False(coldSchema.IsCorrectType<DataSchemaSubSchemaComponent2>());
	}
}