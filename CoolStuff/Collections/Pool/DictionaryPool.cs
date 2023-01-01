namespace Macropus.CoolStuff.Collections.Pool;

public class DictionaryPool<K, V> : APool<Dictionary<K, V>>
{
	public override Dictionary<K, V> Take()
	{
		Interlocked.Increment(ref taken);

		Lock.EnterWriteLock();

		try
		{
			var value = Bag.FirstOrDefault();

			return value ?? new Dictionary<K, V>();
		}
		finally
		{
			if (Lock.IsWriteLockHeld) Lock.ExitWriteLock();
		}
	}

	public override void Release(Dictionary<K, V> obj)
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