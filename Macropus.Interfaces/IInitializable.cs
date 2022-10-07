namespace Macropus.Interfaces;

public interface IInitializable<in T>
{
	Task Initialize(T data, CancellationToken cancellationToken = default);
}