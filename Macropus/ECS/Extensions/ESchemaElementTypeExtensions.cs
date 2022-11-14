using System.Data;
using Macropus.Schema;

namespace Macropus.ECS.Extensions;

public static class ESchemaElementTypeExtensions
{


	public static object Read(this ESchemaElementType type, IDataReader reader, int i)
	{
		switch (type)
		{
			case ESchemaElementType.Int8:
				return (sbyte)reader.GetInt64(i);
			case ESchemaElementType.UInt8:
				return (byte)reader.GetInt64(i);
			case ESchemaElementType.Int16:
				return reader.GetInt16(i);
			case ESchemaElementType.UInt16:
				return (UInt16)reader.GetInt64(i);
			case ESchemaElementType.Int32:
				return reader.GetInt32(i);
			case ESchemaElementType.UInt32:
				return (UInt32)reader.GetInt64(i);
			case ESchemaElementType.Int64:
				return Int64.Parse(reader.GetString(i));
			case ESchemaElementType.UInt64:
				return UInt64.Parse(reader.GetString(i));

			case ESchemaElementType.Int128:
				return Int128.Parse(reader.GetString(i));
			case ESchemaElementType.UInt128:
				return UInt128.Parse(reader.GetString(i));

			case ESchemaElementType.Float:
				return float.Parse(reader.GetString(i));
			case ESchemaElementType.Double:
				return double.Parse(reader.GetString(i));
			case ESchemaElementType.Decimal:
				return decimal.Parse(reader.GetString(i));

			case ESchemaElementType.Guid:
				return Guid.Parse(reader.GetString(i));
			case ESchemaElementType.String:
				return reader.GetString(i);
		}

		throw new ArgumentException($"{nameof(type)} - {type}");
	}

	public static string ToSqlType(this ESchemaElementType type)
	{
		switch (type)
		{
			case ESchemaElementType.Int8:
				return "TINYINT";
			case ESchemaElementType.UInt8:
				return "TINYINT UNSIGNED";
			case ESchemaElementType.Int16:
				return "SMALLINT";
			case ESchemaElementType.UInt16:
				return "SMALLINT UNSIGNED";
			case ESchemaElementType.Int32:
				return "INT";
			case ESchemaElementType.UInt32:
				return "INT UNSIGNED";
			case ESchemaElementType.Int64:
			case ESchemaElementType.UInt64:
			case ESchemaElementType.Int128:
			case ESchemaElementType.UInt128:
			case ESchemaElementType.Float:
			case ESchemaElementType.Double:
			case ESchemaElementType.Decimal:
				return "TEXT";

			case ESchemaElementType.Guid:
				return "TEXT COLLATE NOCASE";
			case ESchemaElementType.String:
				return "TEXT";

			case ESchemaElementType.ComplexType:
				return "INT";
		}

		throw new ArgumentException($"{nameof(type)} - {type}");
	}
}