namespace Macropus.Interfaces;

public interface IInitializableAsync<in T>
{
	Task InitializeAsync(T data, CancellationToken cancellationToken = default);
}