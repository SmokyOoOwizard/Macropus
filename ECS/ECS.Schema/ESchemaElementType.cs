// ReSharper disable InconsistentNaming

namespace ECS.Schema;

public enum ESchemaElementType
{
	INVALID = 0,
	Int8,
	UInt8,
	Int16,
	UInt16,
	Int32,
	UInt32,
	Int64,
	UInt64,
	Int128,
	UInt128,

	Float,
	Double,
	Decimal,

	String,
	Guid,

	ComplexType = int.MaxValue
}