namespace Macropus.CoolStuff.Collections;

public class Pool<T> : APool<T> where T : IClearable, new()
{
	public override T Take()
	{
		Interlocked.Increment(ref taken);

		if (stack.TryPop(out var obj))
		{
			return obj;
		}

		return new T();
	}

	public override void Release(T obj)
	{
		Interlocked.Decrement(ref taken);

		obj.Clear();

		stack.Push(obj);
	}
}