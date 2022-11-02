using System.Text;
using Macropus.Schema;

namespace Macropus.ECS;

public static class DataSchemaElementExtensions
{
	public static string ToSqlInsert(this DataSchemaElement element, object? value)
	{
		if (value == null)
			return "NULL, ";

		switch (element.Info.Type)
		{
			case ESchemaElementType.Guid:
				return $"\'{(Guid)value:N}\', ";
			case ESchemaElementType.String:
				return $"\'{value}\', ";
			default:
				return $"{value}, ";
		}
	}

	public static string ToSqlName(this ColdDataSchemaElement element)
	{
		// TODO replace reserved names (like Id)
		if (element.Type == ESchemaElementType.ComplexType)
			return $"{element.Name}Id";
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