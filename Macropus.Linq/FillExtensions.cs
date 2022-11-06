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
}