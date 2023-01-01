namespace Macropus.CoolStuff.Collections.Pool;

public class Pool<T> : APool<T> where T : IClearable, new()
{
	public override T Take()
	{
		Interlocked.Increment(ref taken);

		Lock.EnterWriteLock();

		try
		{
			var value = Bag.FirstOrDefault();

			return value ?? new T();
		}
		finally
		{
			if (Lock.IsWriteLockHeld) Lock.ExitWriteLock();
		}
	}

	public override void Release(T obj)
	{
		Interlocked.Decrement(ref taken);

		Lock.EnterWriteLock();

		try
		{
			if (Bag.Contains(obj))
				return;

			obj.Clear();

			Bag.Add(obj);
		}
		finally
		{
			if (Lock.IsWriteLockHeld) Lock.ExitWriteLock();
		}
	}
}