using Macropus.ECS.Serialize;
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
		
		for (int i = 0; i < 1; i++)
		{
			var entityId = Guid.NewGuid();
			var testComponent = RandomUtils.GetRandomDataSchemaTestTypeComponent();

			await serializer.SerializeAsync(schema, entityId, testComponent);

			var deserializedComponentNullable =
				await serializer.DeserializeAsync<DataSchemaTestTypeComponent>(schema, entityId);

			Assert.NotNull(deserializedComponentNullable);

			var deserializedComponent = deserializedComponentNullable.Value;

			Assert.Equal(testComponent.ByteField, deserializedComponent.ByteField);
			Assert.Equal(testComponent.SByteField, deserializedComponent.SByteField);

			Assert.Equal(testComponent.UInt16Field, deserializedComponent.UInt16Field);
			Assert.Equal(testComponent.Int16Field, deserializedComponent.Int16Field);

			Assert.Equal(testComponent.UInt32Field, deserializedComponent.UInt32Field);
			Assert.Equal(testComponent.Int32Field, deserializedComponent.Int32Field);

			Assert.Equal(testComponent.UInt64Field, deserializedComponent.UInt64Field);
			Assert.Equal(testComponent.Int64Field, deserializedComponent.Int64Field);

			Assert.Equal(testComponent.UInt128Field, deserializedComponent.UInt128Field);
			Assert.Equal(testComponent.Int128Field, deserializedComponent.Int128Field);

			Assert.Equal(testComponent.FloatField, deserializedComponent.FloatField);
			Assert.Equal(testComponent.DoubleField, deserializedComponent.DoubleField);
			Assert.Equal(testComponent.DecimalField, deserializedComponent.DecimalField);

			Assert.Equal(testComponent.StringField, deserializedComponent.StringField);
			Assert.Equal(testComponent.GuidField, deserializedComponent.GuidField);

			Assert.Equal(testComponent.ComplexField, deserializedComponent.ComplexField);

			Assert.Equal(testComponent.ValueTypeArrayField.Length, deserializedComponent.ValueTypeArrayField.Length);
			for (int j = 0; j < testComponent.ValueTypeArrayField.Length; j++)
			{
				Assert.Equal(testComponent.ValueTypeArrayField[j], deserializedComponent.ValueTypeArrayField[j]);
			}
			
			Assert.Equal(testComponent.ComplexTypeArrayField.Length, deserializedComponent.ComplexTypeArrayField.Length);
			for (int j = 0; j < testComponent.ComplexTypeArrayField.Length; j++)
			{
				Assert.Equal(testComponent.ComplexTypeArrayField[j], deserializedComponent.ComplexTypeArrayField[j]);
			}

			Assert.Equal(testComponent.NullableValueType, deserializedComponent.NullableValueType);
			
			Assert.Equal(testComponent.NullableValueTypeArray.Length, deserializedComponent.NullableValueTypeArray.Length);
			for (int j = 0; j < testComponent.NullableValueTypeArray.Length; j++)
			{
				Assert.Equal(testComponent.NullableValueTypeArray[j], deserializedComponent.NullableValueTypeArray[j]);
			}
			
			Assert.Equal(testComponent.ComplexNullableTypeArrayField.Length, deserializedComponent.ComplexNullableTypeArrayField.Length);
			for (int j = 0; j < testComponent.ComplexNullableTypeArrayField.Length; j++)
			{
				Assert.Equal(testComponent.ComplexNullableTypeArrayField[j], deserializedComponent.ComplexNullableTypeArrayField[j]);
			}
			
			Assert.Equal(testComponent.NamedField, deserializedComponent.NamedField);
		}
	}
}