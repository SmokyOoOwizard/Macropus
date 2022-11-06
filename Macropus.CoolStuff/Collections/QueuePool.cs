namespace Macropus.CoolStuff.Collections;

public class QueuePool<T> : APool<Queue<T>>
{
	public override Queue<T> Take()
	{
		Interlocked.Increment(ref taken);

		if (stack.TryPop(out var obj))
		{
			return obj;
		}

		return new Queue<T>();
	}

	public override void Release(Queue<T> obj)
	{
		Interlocked.Decrement(ref taken);

		obj.Clear();

		stack.Push(obj);
	}
}