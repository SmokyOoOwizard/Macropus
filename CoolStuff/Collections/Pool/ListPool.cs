namespace Macropus.CoolStuff.Collections.Pool;

public class ListPool<T> : APool<List<T>>
{
	public static ListPool<T> Instance { get; } = new();

	public override List<T> Take()
	{
		Interlocked.Increment(ref taken);

		Lock.EnterWriteLock();

		try
		{
			var value = Bag.FirstOrDefault();
			if (value != null)
				Bag.Remove(value);

			return value ?? new List<T>();
		}
		finally
		{
			if (Lock.IsWriteLockHeld) Lock.ExitWriteLock();
		}
	}

	public override void Release(List<T> obj)
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		if (obj == null)
			return;
		
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