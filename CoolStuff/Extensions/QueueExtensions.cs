namespace Macropus.Extensions;

public static class QueueExtensions
{
	public static Queue<T> AddRange<T>(this Queue<T> collection, IEnumerable<T> collection2)
	{
		foreach (var item in collection2)
		{ 
			collection.Enqueue(item);
		}

		return collection;
	}
}