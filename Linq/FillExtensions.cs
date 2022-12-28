namespace Macropus.Linq;

public static class FillExtensions
{
	public static void Fill<T>(this IEnumerable<T> enumerable, ICollection<T> target)
	{
		foreach (var item in enumerable)
		{
			target.Add(item);
		}
	}
	
	public static void Fill<T>(this IEnumerable<T> enumerable, ICollection<T> target, int count)
	{
		var i = 0;
		foreach (var item in enumerable)
		{
			if (i >= count)
				break;
			
			target.Add(item);
			i++;
		}
	}
	
	public static void Fill<T>(this IEnumerable<T> enumerable, Stack<T> target)
	{
		foreach (var item in enumerable)
		{
			target.Push(item);
		}
	}
}