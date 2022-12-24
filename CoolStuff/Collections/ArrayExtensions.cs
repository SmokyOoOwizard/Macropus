namespace Macropus.CoolStuff.Collections;

public static class ArrayExtensions
{
	private static readonly Dictionary<Type, object> EmptyArrays = new();

	public static object Empty(Type type)
	{
		if (EmptyArrays.TryGetValue(type, out var emptyArray))
			return emptyArray;

		lock (EmptyArrays)
		{
			if (EmptyArrays.TryGetValue(type, out emptyArray))
				return emptyArray;

			EmptyArrays[type] = Array.CreateInstance(type, 0);
			
			return EmptyArrays[type];
		}
	}
}