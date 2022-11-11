namespace Macropus.CoolStuff.Collections.Pool;

public class ListPool<T> : APool<List<T>>
{
	public override List<T> Take()
	{
		Interlocked.Increment(ref taken);

		if (stack.TryPop(out var obj))
			return obj;

		return new List<T>();
	}

	public override void Release(List<T> obj)
	{
		Interlocked.Decrement(ref taken);

		obj.Clear();

		stack.Push(obj);
	}
}