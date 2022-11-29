namespace Macropus.CoolStuff;

public interface IMakeRef<out T>
{
	T MakeRef();
}