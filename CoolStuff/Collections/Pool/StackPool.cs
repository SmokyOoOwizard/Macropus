namespace Macropus.CoolStuff.Collections.Pool;

public class StackPool<T> : APool<Stack<T>>
{
	public override Stack<T> Take()
	{
		Interlocked.Increment(ref taken);
		
		Lock.EnterWriteLock();

		try
		{
			var value = Bag.FirstOrDefault();

			return value ?? new Stack<T>();
		}
		finally
		{
			if (Lock.IsWriteLockHeld) Lock.ExitWriteLock();
		}
	}

	public override void Release(Stack<T> obj)
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