using System.Collections.Concurrent;

namespace Macropus.CoolStuff.Collections.Pool;

public abstract class APool<T> : IPool<T>
{
	protected readonly ConcurrentStack<T> stack = new();

	protected int taken = 0;

	public int Taken => taken;

	public int ObjectsInPool => stack.Count;

	object IPool.Take()
	{
		return Take();
	}

	public void Release(object obj)
	{
		if (obj is not T objPool)
			throw new InvalidCastException();

		Release(objPool);
	}

	public abstract T Take();
	public abstract void Release(T obj);
}