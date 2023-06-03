using System.Text;
using ECS.Schema;

namespace ECS.Serialize.Extensions;

public static class DataSchemaElementExtensions
{
	public static object? ToSqlInsert(this DataSchemaElement element, object? value)
	{
		if (value == null)
			return null;

		switch (element.Info.Type)
		{
			case ESchemaElementType.Guid:
				return $"{(Guid)value:N}";

			case ESchemaElementType.Int64:
			case ESchemaElementType.UInt64:
			case ESchemaElementType.Int128:
			case ESchemaElementType.UInt128:
			case ESchemaElementType.Float:
			case ESchemaElementType.Double:
			case ESchemaElementType.Decimal:
			case ESchemaElementType.String:
				return $"{value}";
			default:
				return value;
		}
	}

	public static string ToSqlName(this ColdDataSchemaElement element)
	{
		return element.Name;
	}

	public static IEnumerable<string> ToSql(this IEnumerable<DataSchemaElement> elements)
	{
		var stringBuilder = new StringBuilder();
		foreach (var element in elements)
		{
			var info = element.Info;

			stringBuilder.Append($"{info.ToSqlName()} ");

			if (info.CollectionType is ECollectionType.Array)
			{
				stringBuilder.Append("ARRAY[");
				stringBuilder.Append($"{info.Type.ToSqlType()} ");
				if (!info.Nullable)
					stringBuilder.Append("NOT NULL ");
				stringBuilder.Append("] ");
			}
			else
			{
				stringBuilder.Append($"{info.Type.ToSqlType()} ");
				if (!info.Nullable)
					stringBuilder.Append("NOT NULL ");
			}

			yield return stringBuilder.ToString();

			stringBuilder.Clear();
		}
	}
}