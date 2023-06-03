using System.Data;
using ECS.Schema;

namespace ECS.Serialize.Extensions;

public static class ESchemaElementTypeExtensions
{
	public static object Read(this ESchemaElementType type, IDataReader reader, int i)
	{
		return type switch
		{
			ESchemaElementType.Int8 => (sbyte)reader.GetInt64(i),
			ESchemaElementType.UInt8 => (byte)reader.GetInt64(i),
			ESchemaElementType.Int16 => reader.GetInt16(i),
			ESchemaElementType.UInt16 => (UInt16)reader.GetInt64(i),
			ESchemaElementType.Int32 => reader.GetInt32(i),
			ESchemaElementType.UInt32 => (UInt32)reader.GetInt64(i),
			ESchemaElementType.Int64 => Int64.Parse(reader.GetString(i)),
			ESchemaElementType.UInt64 => UInt64.Parse(reader.GetString(i)),
			ESchemaElementType.Int128 => Int128.Parse(reader.GetString(i)),
			ESchemaElementType.UInt128 => UInt128.Parse(reader.GetString(i)),
			ESchemaElementType.Float => float.Parse(reader.GetString(i)),
			ESchemaElementType.Double => double.Parse(reader.GetString(i)),
			ESchemaElementType.Decimal => decimal.Parse(reader.GetString(i)),
			ESchemaElementType.Guid => Guid.Parse(reader.GetString(i)),
			ESchemaElementType.String => reader.GetString(i),
			_ => throw new ArgumentException($"{nameof(type)} - {type}")
		};
	}

	public static string ToSqlType(this ESchemaElementType type)
	{
		return type switch
		{
			ESchemaElementType.Int8 => "TINYINT",
			ESchemaElementType.UInt8 => "TINYINT UNSIGNED",
			ESchemaElementType.Int16 => "SMALLINT",
			ESchemaElementType.UInt16 => "SMALLINT UNSIGNED",
			ESchemaElementType.Int32 => "INT",
			ESchemaElementType.UInt32 => "INT UNSIGNED",
			ESchemaElementType.Int64 => "TEXT",
			ESchemaElementType.UInt64 => "TEXT",
			ESchemaElementType.Int128 => "TEXT",
			ESchemaElementType.UInt128 => "TEXT",
			ESchemaElementType.Float => "TEXT",
			ESchemaElementType.Double => "TEXT",
			ESchemaElementType.Decimal => "TEXT",
			ESchemaElementType.Guid => "TEXT COLLATE NOCASE",
			ESchemaElementType.String => "TEXT",
			ESchemaElementType.ComplexType => "TEXT",
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}
}