using ECS.Schema.Attributes;
using Macropus.ECS.Component;

namespace ECS.Tests.Schema;

public struct DataSchemaSubSchemaComponent
{
	public DataSchemaSubSchemaComponent2 InnerStructure;
}

public struct DataSchemaSubSchemaComponent2
{
	public string Name;
}

[Name("Schema")]
public struct DataSchemaTestTypeComponent : IComponent
{
	public byte ByteField;
	public sbyte SByteField;

	public ushort UInt16Field;
	public short Int16Field;

	public uint UInt32Field;
	public int Int32Field;

	public ulong UInt64Field;
	public long Int64Field;

	public UInt128 UInt128Field;
	public Int128 Int128Field;

	public float FloatField;
	public double DoubleField;
	public decimal DecimalField;

	public string StringField;
	public Guid GuidField;

	public DataSchemaSubSchemaComponent ComplexField;

	public float[] ValueTypeArrayField;
	public DataSchemaSubSchemaComponent[] ComplexTypeArrayField;

	public ulong? NullableValueType;
	public ulong?[] NullableValueTypeArray;
	public DataSchemaSubSchemaComponent?[] ComplexNullableTypeArrayField;
	
	public DataSchemaSubSchemaComponent2?[] SimpleComplexNullableTypeArrayField;
	public DataSchemaSubSchemaComponent2[] SimpleComplexTypeArrayField;

	[Name("IsCommonField")]
	public float NamedField;
}