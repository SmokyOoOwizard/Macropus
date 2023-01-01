namespace Macropus.CoolStuff.Collections.Pool;

public class QueuePool<T> : APool<Queue<T>>
{
	public override Queue<T> Take()
	{
		Interlocked.Increment(ref taken);

		Lock.EnterWriteLock();

		try
		{
			var value = Bag.FirstOrDefault();

			return value ?? new Queue<T>();
		}
		finally
		{
			if (Lock.IsWriteLockHeld) Lock.ExitWriteLock();
		}
	}

	public override void Release(Queue<T> obj)
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