namespace Macropus.CoolStuff.Collections;

public class ListPool<T> : APool<List<T>> where T :  new()
{
	public override List<T> Take()
	{
		Interlocked.Increment(ref taken);

		if (stack.TryPop(out var obj))
		{
			return obj;
		}

		return new List<T>();
	}

	public override void Release(List<T> obj)
	{
		Interlocked.Decrement(ref taken);

		obj.Clear();

		stack.Push(obj);
	}
}