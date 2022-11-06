namespace Macropus.CoolStuff.Collections;

public class StackPool<T> : APool<Stack<T>> where T :  new()
{
	public override Stack<T> Take()
	{
		Interlocked.Increment(ref taken);

		if (stack.TryPeek(out var obj))
		{
			return obj;
		}

		return new Stack<T>();
	}

	public override void Release(Stack<T> obj)
	{
		Interlocked.Decrement(ref taken);

		obj.Clear();

		stack.Push(obj);
	}
}