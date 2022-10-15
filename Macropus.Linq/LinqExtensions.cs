using System.Diagnostics.Contracts;

namespace Macropus.Linq;

public static class LinqExtensions
{
	[Pure]
	public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : class
	{
		return enumerable.Where(e => e != null).Select(e => e!);
	}

	[Pure]
	public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : struct
	{
		return enumerable.Where(e => e.HasValue).Select(e => e!.Value);
	}
}