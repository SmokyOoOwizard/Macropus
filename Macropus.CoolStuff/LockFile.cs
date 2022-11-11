namespace Macropus.CoolStuff;

public sealed class LockFile : IDisposable, IAsyncDisposable
{
	private readonly FileStream lockFile;

	private bool disposed;

	private LockFile(FileStream lockFile)
	{
		this.lockFile = lockFile;
	}


	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		GC.SuppressFinalize(this);

		lockFile.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		if (disposed) return;
		disposed = true;

		GC.SuppressFinalize(this);

		await lockFile.DisposeAsync();
	}

	~LockFile()
	{
		Dispose();
	}

	public static async Task<LockFile> LockWhileAsync(string path, string lockName = nameof(lockFile),
		CancellationToken cancellationToken = default)
	{
		path = Path.Combine(path, lockName);

		return await Task.Run(async () =>
		{
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();
				try
				{
					var steam = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);

					return new LockFile(steam);
				}
				catch
				{
					// ignored
				}

				await Task.Delay(10, cancellationToken);
			}
		}, cancellationToken).ConfigureAwait(false);
	}

	public static bool TryLock(string path, out LockFile lockFile)
	{
		try
		{
			var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);

			lockFile = new LockFile(stream);
			return true;
		}
		catch
		{
			lockFile = default!;
			return false;
		}
	}
}