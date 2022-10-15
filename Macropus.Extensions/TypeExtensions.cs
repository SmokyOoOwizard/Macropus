namespace Macropus.Extensions;

public static class TypeExtensions
{
	public static bool IsStruct(this Type type)
	{
		if (type.IsValueType)
			//Is Value Type
			if (!type.IsPrimitive)
				/* Is not primitive. Remember that primitives are:
				Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32,
				Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single.
				This way, could still be Decimal, Date or Enum. */
				if (type != typeof(decimal))
					//Is not Decimal
					if (type != typeof(DateTime))
						//Is not Date
						if (!type.IsEnum)
							//Is not Enum. Consequently it is a structure.
							return true;

		return false;
	}
}