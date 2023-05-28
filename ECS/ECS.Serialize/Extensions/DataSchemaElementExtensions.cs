using System.Text;
using ECS.Schema;
using ECS.Schema.Extensions;

namespace ECS.Serialize.Extensions;

public static class DataSchemaElementExtensions
{
	public static string ToSqlArrayInsert(this DataSchemaElement element, object? value)
	{
		if (value == null)
			return "null, ";

		switch (element.Info.Type)
		{
			case ESchemaElementType.Guid:
				return $"'{(Guid)value:N}', ";

			case ESchemaElementType.Int64:
			case ESchemaElementType.UInt64:
			case ESchemaElementType.Int128:
			case ESchemaElementType.UInt128:
			case ESchemaElementType.Float:
			case ESchemaElementType.Double:
			case ESchemaElementType.Decimal:
			case ESchemaElementType.String:
				return $"'{value}', ";
			default:
				return $"{value}, ";
		}
	}

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
		// TODO replace reserved names (like Id)
		if (element.Type == ESchemaElementType.ComplexType)
			return $"{element.Name}Id";
		return element.Name;
	}

	public static object? Parse(this ColdDataSchemaElement type, string? rawObj)
	{
		if (type.Nullable)
		{
			switch (type.Type)
			{
				case ESchemaElementType.Int8:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(sbyte?);

					return sbyte.Parse(rawObj);
				case ESchemaElementType.UInt8:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(byte?);

					return byte.Parse(rawObj);
				case ESchemaElementType.Int16:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(Int16?);

					return Int16.Parse(rawObj);
				case ESchemaElementType.UInt16:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(UInt16?);

					return UInt16.Parse(rawObj);
				case ESchemaElementType.Int32:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(Int32?);

					return Int32.Parse(rawObj);
				case ESchemaElementType.UInt32:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(UInt32?);

					return UInt32.Parse(rawObj);
				case ESchemaElementType.Int64:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(Int64?);

					return Int64.Parse(rawObj);
				case ESchemaElementType.UInt64:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(UInt64?);

					return UInt64.Parse(rawObj);
				case ESchemaElementType.Int128:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(Int128?);

					return Int128.Parse(rawObj);
				case ESchemaElementType.UInt128:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(UInt128?);

					return UInt128.Parse(rawObj);


				case ESchemaElementType.Float:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(float?);

					return float.Parse(rawObj);
				case ESchemaElementType.Double:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(double?);

					return double.Parse(rawObj);
				case ESchemaElementType.Decimal:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(decimal?);

					return decimal.Parse(rawObj);

				case ESchemaElementType.Guid:
					if (string.IsNullOrWhiteSpace(rawObj))
						return default(Guid?);

					return Guid.Parse(rawObj);
				case ESchemaElementType.String:
					return rawObj;
			}
		}

		switch (type.Type)
		{
			case ESchemaElementType.Int8:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(sbyte);

				return sbyte.Parse(rawObj);
			case ESchemaElementType.UInt8:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(byte);

				return byte.Parse(rawObj);
			case ESchemaElementType.Int16:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(Int16);

				return Int16.Parse(rawObj);
			case ESchemaElementType.UInt16:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(UInt16);

				return UInt16.Parse(rawObj);
			case ESchemaElementType.Int32:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(Int32);

				return Int32.Parse(rawObj);
			case ESchemaElementType.UInt32:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(UInt32);

				return UInt32.Parse(rawObj);
			case ESchemaElementType.Int64:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(Int64);

				return Int64.Parse(rawObj);
			case ESchemaElementType.UInt64:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(UInt64);

				return UInt64.Parse(rawObj);
			case ESchemaElementType.Int128:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(Int128);

				return Int128.Parse(rawObj);
			case ESchemaElementType.UInt128:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(UInt128);

				return UInt128.Parse(rawObj);


			case ESchemaElementType.Float:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(float);

				return float.Parse(rawObj);
			case ESchemaElementType.Double:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(double);

				return double.Parse(rawObj);
			case ESchemaElementType.Decimal:
				if (string.IsNullOrWhiteSpace(rawObj))
					return default(decimal);

				return decimal.Parse(rawObj);

			case ESchemaElementType.Guid:
				if (string.IsNullOrWhiteSpace(rawObj))
					return Guid.Empty;

				return Guid.Parse(rawObj);
			case ESchemaElementType.String:
				return rawObj;
		}

		throw new ArgumentException($"{nameof(type)} - {type}");
	}

	public static Type ToType(this DataSchemaElement element)
	{
		var type = element.Info.Type;
		if (element.Info.Nullable)
		{
			switch (type)
			{
				case ESchemaElementType.Int8:
					return typeof(sbyte?);
				case ESchemaElementType.UInt8:
					return typeof(byte?);
				case ESchemaElementType.Int16:
					return typeof(Int16?);
				case ESchemaElementType.UInt16:
					return typeof(UInt16?);
				case ESchemaElementType.Int32:
					return typeof(Int32?);
				case ESchemaElementType.UInt32:
					return typeof(UInt32?);
				case ESchemaElementType.Int64:
					return typeof(Int64?);
				case ESchemaElementType.UInt64:
					return typeof(UInt64?);

				case ESchemaElementType.Int128:
					return typeof(Int128?);
				case ESchemaElementType.UInt128:
					return typeof(UInt128?);

				case ESchemaElementType.Float:
					return typeof(float?);
				case ESchemaElementType.Double:
					return typeof(double?);
				case ESchemaElementType.Decimal:
					return typeof(decimal?);

				case ESchemaElementType.Guid:
					return typeof(Guid?);
				case ESchemaElementType.String:
					return typeof(string);
			}
		}

		switch (type)
		{
			case ESchemaElementType.Int8:
				return typeof(sbyte);
			case ESchemaElementType.UInt8:
				return typeof(byte);
			case ESchemaElementType.Int16:
				return typeof(Int16);
			case ESchemaElementType.UInt16:
				return typeof(UInt16);
			case ESchemaElementType.Int32:
				return typeof(Int32);
			case ESchemaElementType.UInt32:
				return typeof(UInt32);
			case ESchemaElementType.Int64:
				return typeof(Int64);
			case ESchemaElementType.UInt64:
				return typeof(UInt64);

			case ESchemaElementType.Int128:
				return typeof(Int128);
			case ESchemaElementType.UInt128:
				return typeof(UInt128);

			case ESchemaElementType.Float:
				return typeof(float);
			case ESchemaElementType.Double:
				return typeof(double);
			case ESchemaElementType.Decimal:
				return typeof(decimal);

			case ESchemaElementType.Guid:
				return typeof(Guid);
			case ESchemaElementType.String:
				return typeof(string);
		}

		if (element.Info.Type == ESchemaElementType.ComplexType)
		{
			return element.FieldInfo.FieldType.GetSchemaRealType();
		}

		throw new ArgumentException($"{nameof(type)} - {type}");
	}

	public static IEnumerable<string> ToSqlName(this IEnumerable<DataSchemaElement> elements)
	{
		foreach (var element in elements)
		{
			yield return element.Info.ToSqlName();
		}
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