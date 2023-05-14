using System.Collections;

namespace Macropus.Linq;

public static class AnyExtensions
{
	public static bool Any(this IEnumerable source)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source));

		if (source is ICollection collection)
			return collection.Count != 0;

		return source.GetEnumerator().MoveNext();
	}
}