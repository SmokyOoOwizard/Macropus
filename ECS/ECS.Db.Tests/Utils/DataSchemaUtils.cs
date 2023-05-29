using ECS.Tests.Schema;

namespace ECS.Db.Tests.Utils;

public static class DataSchemaUtils
{
	public static void CheckDeserializedComponent(
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