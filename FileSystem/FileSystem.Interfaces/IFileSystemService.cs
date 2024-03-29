﻿namespace Macropus.FileSystem.Interfaces;

public interface IFileSystemService : IDisposable, IAsyncDisposable
{
	Task<IFileProvider> GetFileAsync(
		Guid id,
		FileAccess access,
		FileShare share,
		CancellationToken cancellationToken = default
	);

	Task<Guid> CreateFileAsync(string name, CancellationToken cancellationToken = default);
	Task DeleteFileAsync(Guid id, CancellationToken cancellationToken = default);
}