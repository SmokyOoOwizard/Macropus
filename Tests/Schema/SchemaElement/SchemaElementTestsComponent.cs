using Macropus.ECS.Component;
using Macropus.Schema.Attributes;

namespace Tests.Schema.SchemaElement;

public struct TestStructure
{
	public TestStructure2 InnerStructure;
}

public struct TestStructure2
{
	public string Name;
}

[Name("Schema")]
public struct SchemaElementTestsComponent : IComponent
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

	public TestStructure ComplexField;

	public float[] ValueTypeArrayField;
	public TestStructure[] ComplexTypeArrayField;

	[Name("IsCommonField")]
	public float NamedField;

	public static SchemaElementTestsComponent CreateAndFillRandomData()
	{
		throw new NotImplementedException();
	}
}