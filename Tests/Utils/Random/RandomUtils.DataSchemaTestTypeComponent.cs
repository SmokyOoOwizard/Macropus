using Tests.Schema;

namespace Tests.Utils.Random;

public static partial class RandomUtils
{
	public static DataSchemaTestTypeComponent GetRandomDataSchemaTestTypeComponent()
	{
		var component = new DataSchemaTestTypeComponent();
		component.ByteField = RandomUtils.GetRandomByte();
		component.SByteField = RandomUtils.GetRandomSByte();

		component.UInt16Field = RandomUtils.GetRandomUInt16();
		component.Int16Field = RandomUtils.GetRandomInt16();

		component.UInt32Field = RandomUtils.GetRandomUInt();
		component.Int32Field = RandomUtils.GetRandomInt();

		component.UInt64Field = RandomUtils.GetRandomUInt64();
		component.Int64Field = RandomUtils.GetRandomInt64();

		component.UInt128Field = RandomUtils.GetRandomUInt128();
		component.Int128Field = RandomUtils.GetRandomInt128();

		component.FloatField = RandomUtils.GetRandomFloat();
		component.DoubleField = RandomUtils.GetRandomDouble();
		component.DecimalField = RandomUtils.GetRandomDecimal();

		component.StringField = RandomUtils.GetRandomString(64);
		component.GuidField = Guid.NewGuid();

		component.ComplexField.InnerStructure.Name = RandomUtils.GetRandomString(64);

		component.ValueTypeArrayField = RandomUtils.GetRandomFloatArray(64);

		var complexTypeArrayLength = RandomUtils.GetRandomByte();
		component.ComplexTypeArrayField = new DataSchemaSubSchemaComponent[complexTypeArrayLength];
		for (int i = 0; i < complexTypeArrayLength; i++)
		{
			component.ComplexTypeArrayField[i].InnerStructure.Name = RandomUtils.GetRandomString(64);
		}
		

		if (RandomUtils.GetRandomBool())
		{
			component.NullableValueType = RandomUtils.GetRandomUInt64();
		}

		var nullableValueTypeArrayLength = RandomUtils.GetRandomByte();
		component.NullableValueTypeArray = new ulong?[nullableValueTypeArrayLength];
		for (int i = 0; i < nullableValueTypeArrayLength; i++)
		{
			if (!RandomUtils.GetRandomBool())
				continue;
			
			component.NullableValueTypeArray[i] = RandomUtils.GetRandomUInt64();
		}

		var complexNullableTypeArrayFieldLength = RandomUtils.GetRandomByte();
		component.ComplexNullableTypeArrayField =
			new DataSchemaSubSchemaComponent?[complexNullableTypeArrayFieldLength];
		for (int i = 0; i < complexNullableTypeArrayFieldLength; i++)
		{
			if (!RandomUtils.GetRandomBool())
				continue;
			
			var st = new DataSchemaSubSchemaComponent();
			st.InnerStructure.Name = RandomUtils.GetRandomString(64);
			component.ComplexNullableTypeArrayField[i] = st;
		}
		
		var simpleComplexNullableTypeArrayFieldLength = RandomUtils.GetRandomByte();
		component.SimpleComplexNullableTypeArrayField = new DataSchemaSubSchemaComponent2?[simpleComplexNullableTypeArrayFieldLength];
		for (int i = 0; i < simpleComplexNullableTypeArrayFieldLength; i++)
		{
			if (!RandomUtils.GetRandomBool())
				continue;
			
			var st = new DataSchemaSubSchemaComponent2
			{
				Name = RandomUtils.GetRandomString(64)
			};
			component.SimpleComplexNullableTypeArrayField[i] = st;
		}

		component.NamedField = RandomUtils.GetRandomFloat();

		return component;
	}
}