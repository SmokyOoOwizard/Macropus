namespace Macropus.Extensions;

public static class DisposableExtensions
{
	public static void TryDispose(this IDisposable disposable)
	{
		try
		{
			disposable.Dispose();
		}
		catch
		{
			// ignored
		}
	}

	public static void AddTo(this IDisposable disposable, ICollection<IDisposable> collection)
	{
		collection.Add(disposable);
	}
}