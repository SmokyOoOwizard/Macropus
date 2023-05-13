namespace Macropus.CoolStuff;

public interface IInitializableAsync<in T>
{
	Task InitializeAsync(T data, CancellationToken cancellationToken = default);
}

public interface IInitializableAsync
{
	Task InitializeAsync(CancellationToken cancellationToken = default);
}