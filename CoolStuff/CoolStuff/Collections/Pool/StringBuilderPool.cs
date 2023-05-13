using System.Text;

namespace Macropus.CoolStuff.Collections.Pool;

public class StringBuilderPool : APool<StringBuilder>
{
	public static StringBuilderPool Instance { get; } = new();
	
	public override StringBuilder Take()
	{
		Interlocked.Increment(ref taken);

		Lock.EnterWriteLock();

		try
		{
			var value = Bag.FirstOrDefault();
			if (value != null)
				Bag.Remove(value);

			return value ?? new StringBuilder();
		}
		finally
		{
			if (Lock.IsWriteLockHeld) Lock.ExitWriteLock();
		}
	}

	public override void Release(StringBuilder obj)
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