using System.Diagnostics.Contracts;

namespace Macropus.Extensions;

public static class ArrayExtensions
{
	[Pure]
	public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this T[] collection)
	{
		return collection;
	}
}