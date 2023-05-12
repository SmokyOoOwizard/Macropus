using System.Diagnostics;
using ECS.Serialize;
using Macropus.Schema;
using Tests.Utils.Random;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Schema.Serializer;

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

		var components = new List<DataSchemaTestTypeComponent>();
		components.Add(new DataSchemaTestTypeComponent());
		components.Add(new DataSchemaTestTypeComponent
		{
			ComplexTypeArrayField = new DataSchemaSubSchemaComponent[0]
		});
		components.Add(new DataSchemaTestTypeComponent
		{
			SimpleComplexTypeArrayField = new DataSchemaSubSchemaComponent2[1000]
		});

		for (int i = 0; i < 300; i++)
		{
			components.Add(RandomUtils.GetRandomDataSchemaTestTypeComponent());
		}

		var stopwatch = new Stopwatch();

		foreach (var component in components)
		{
			var entityId = Guid.NewGuid();

			stopwatch.Restart();
			await serializer.SerializeAsync(schema, entityId, component);
			stopwatch.Stop();
			
			Output.WriteLine($"Serialize: id: {entityId:N} \ttime: {stopwatch.Elapsed.ToString("c")}");
			
			stopwatch.Restart();
			var deserializedComponentNullable = await serializer.DeserializeAsync<DataSchemaTestTypeComponent>(schema, entityId);
			stopwatch.Stop();
			
			Output.WriteLine($"Deserialize: id: {entityId:N} \ttime: {stopwatch.Elapsed.ToString("c")}\n");
			
			CheckDeserializedComponent(deserializedComponentNullable, component);
		}
	}

	private static void CheckDeserializedComponent(
		DataSchemaTestTypeComponent? deserializedComponentNullable,
		DataSchemaTestTypeComponent testComponent
	)
	{
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

		CheckArray(testComponent.ValueTypeArrayField, deserializedComponent.ValueTypeArrayField);
		CheckArray(testComponent.ComplexTypeArrayField, deserializedComponent.ComplexTypeArrayField);

		Assert.Equal(testComponent.NullableValueType, deserializedComponent.NullableValueType);

		CheckArray(testComponent.NullableValueTypeArray, deserializedComponent.NullableValueTypeArray);
		CheckArray(testComponent.ComplexNullableTypeArrayField, deserializedComponent.ComplexNullableTypeArrayField);
		CheckArray(testComponent.SimpleComplexNullableTypeArrayField, deserializedComponent.SimpleComplexNullableTypeArrayField);
		CheckArray(testComponent.SimpleComplexTypeArrayField, deserializedComponent.SimpleComplexTypeArrayField);

		Assert.Equal(testComponent.NamedField, deserializedComponent.NamedField);
	}

	private static void CheckArray<T>(T[]? expected, T[]? actual)
	{
		if (expected == null)
			Assert.Null(actual);
		else
			Assert.NotNull(actual);

		if (expected != null && actual != null)
		{
			Assert.Equal(expected.Length, actual.Length);
			for (int j = 0; j < expected.Length; j++)
			{
				Assert.Equal(expected[j], actual[j]);
			}
		}
	}
}