namespace Macropus.CoolStuff.Collections.Pool;

public class StackPool<T> : APool<Stack<T>>
{
	public override Stack<T> Take()
	{
		Interlocked.Increment(ref taken);

		if (stack.TryPop(out var obj))
			return obj;

		return new Stack<T>();
	}

	public override void Release(Stack<T> obj)
	{
		Interlocked.Decrement(ref taken);

		obj.Clear();

		stack.Push(obj);
	}
}