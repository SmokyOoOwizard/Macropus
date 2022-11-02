using Macropus.Schema;

namespace Macropus.ECS;

public static class ESchemaElementTypeExtensions
{
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
				return "BIGINT";
			case ESchemaElementType.UInt64:
				return "BIGINT UNSIGNED";

			case ESchemaElementType.Int128:
			case ESchemaElementType.UInt128:
				return "BINARY(16)";

			case ESchemaElementType.Float:
				return "FLOAT";
			case ESchemaElementType.Double:
				return "DOUBLE";
			case ESchemaElementType.Decimal:
				return "DECIMAL";

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