namespace Macropus.Extensions;

public static class TypeExtensions
{
	public static bool IsStruct(this Type type)
	{
		return type.IsValueType
		       && !type.IsPrimitive
		       && type != typeof(decimal)
		       && type != typeof(DateTime)
		       && !type.IsEnum;
	}
}