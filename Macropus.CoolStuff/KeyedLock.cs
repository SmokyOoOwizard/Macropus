using System.Collections.Concurrent;
using Nito.AsyncEx;

namespace Macropus.CoolStuff;

public class KeyedLock<T> where T : notnull
{
	private readonly ConcurrentDictionary<T, AsyncLock> locks = new();

	public IDisposable Lock(T key)
	{
		return locks.GetOrAdd(key, _ => new AsyncLock()).Lock();
	}
}