using System.Diagnostics.Contracts;

namespace Macropus.Linq;

public static class ArrayExtensions
{
	[Pure]
	public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this T[] collection)
	{
		return collection;
	}
}