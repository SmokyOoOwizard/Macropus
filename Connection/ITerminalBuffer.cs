namespace Connection;

public interface ITerminalBuffer<T>
{
	T this[int x, int y] { get; set; }

	void Clear();
}