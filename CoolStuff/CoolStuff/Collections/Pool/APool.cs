namespace Macropus.CoolStuff.Collections.Pool;

public abstract class APool<T> : IPool<T>
{
	protected readonly ReaderWriterLockSlim Lock = new(LockRecursionPolicy.SupportsRecursion);
	protected readonly HashSet<T> Bag = new();

	protected int taken = 0;

	public int Taken => taken;

	public int ObjectsInPool
	{
		get
		{
			Lock.EnterReadLock();

			try
			{
				return Bag.Count;
			}
			finally
			{
				Lock.ExitReadLock();
			}
		}
	}

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