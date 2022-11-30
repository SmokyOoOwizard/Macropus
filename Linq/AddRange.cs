namespace Macropus.Linq;

public static class AddRangeExtensions
{
	public static HashSet<T> AddRange<T>(this HashSet<T> set, ICollection<T> collection)
	{
		foreach (var item in collection)
		{
			set.Add(item);
		}

		return set;
	}
}