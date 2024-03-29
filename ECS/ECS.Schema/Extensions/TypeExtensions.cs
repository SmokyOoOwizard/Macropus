﻿
using Macropus.Extensions;

namespace ECS.Schema.Extensions;

public static class TypeExtensions
{
	public static bool FilterDataSchemaElement(this Type type)
	{
		if (type.GetSchemaType() != ESchemaElementType.INVALID)
			return true;

		if (type.IsArray) // Array
		{
			var elementType = type.GetElementType();
			if (elementType.GetSchemaType() != ESchemaElementType.INVALID)
				return true;
		}

		if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>))) // Nullable
		{
			var nullableType = Nullable.GetUnderlyingType(type);

			if (nullableType != null)
				return FilterDataSchemaElement(nullableType);
		}

		return false;
	}

	public static Type? GetSchemaRealType(this Type? type)
	{
		if (type == null)
			return null;

		if (type.IsArray)
			type = type.GetElementType();

		if (type == null)
			return null;

		if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
			return Nullable.GetUnderlyingType(type);

		return type;
	}

	public static ESchemaElementType GetSchemaType(this Type? type)
	{
		if (type == null)
			return ESchemaElementType.INVALID;

		if (type.IsArray)
			type = type.GetElementType();

		if (type == null)
			return ESchemaElementType.INVALID;

		if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
			type = Nullable.GetUnderlyingType(type);

		if (type == null)
			return ESchemaElementType.INVALID;

		switch (type)
		{
			case var x when x == typeof(byte):
				return ESchemaElementType.UInt8;
			case var x when x == typeof(sbyte):
				return ESchemaElementType.Int8;
			case var x when x == typeof(short):
				return ESchemaElementType.Int16;
			case var x when x == typeof(ushort):
				return ESchemaElementType.UInt16;
			case var x when x == typeof(int):
				return ESchemaElementType.Int32;
			case var x when x == typeof(uint):
				return ESchemaElementType.UInt32;
			case var x when x == typeof(long):
				return ESchemaElementType.Int64;
			case var x when x == typeof(ulong):
				return ESchemaElementType.UInt64;
			case var x when x == typeof(Int128):
				return ESchemaElementType.Int128;
			case var x when x == typeof(UInt128):
				return ESchemaElementType.UInt128;

			case var x when x == typeof(float):
				return ESchemaElementType.Float;
			case var x when x == typeof(double):
				return ESchemaElementType.Double;
			case var x when x == typeof(decimal):
				return ESchemaElementType.Decimal;

			case var x when x == typeof(string):
				return ESchemaElementType.String;
			case var x when x == typeof(Guid):
				return ESchemaElementType.Guid;

			case var x when x.IsStruct():
				return ESchemaElementType.ComplexType;
		}

		return ESchemaElementType.INVALID;
	}
}