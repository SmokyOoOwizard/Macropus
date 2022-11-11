namespace Macropus.CoolStuff.Collections.Pool;

public class DictionaryPool<K, V> : APool<Dictionary<K, V>>
{
	public override Dictionary<K, V> Take()
	{
		Interlocked.Increment(ref taken);

		if (stack.TryPop(out var obj))
			return obj;

		return new Dictionary<K, V>();
	}

	public override void Release(Dictionary<K, V> obj)
	{
		Interlocked.Decrement(ref taken);

		obj.Clear();

		stack.Push(obj);
	}
}