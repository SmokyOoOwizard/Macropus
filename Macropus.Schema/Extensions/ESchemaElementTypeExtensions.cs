namespace Macropus.Schema.Extensions;

public static class ESchemaElementTypeExtensions
{
	public static bool IsSimpleType(this ESchemaElementType type)
	{
		switch (type)
		{
			case ESchemaElementType.ComplexType:
			case ESchemaElementType.INVALID:
				return false;
		}

		return true;
	}
}