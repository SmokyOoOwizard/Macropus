namespace Macropus.CoolStuff.Collections;

interface IPool
{
	int Taken { get; }
	int ObjectsInPool { get; }

	object Take();

	void Release(object obj);
}

interface IPool<T> : IPool
{
	new T Take();

	void Release(T obj);
}