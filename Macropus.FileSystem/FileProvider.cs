using Macropus.FileSystem.Interfaces;

namespace Macropus.FileSystem;

internal class FileProvider : IFileProviderInternal
{
	private readonly FileStream fileStream;

	public string Path { get; }

	public string? Name { get; }

	public Guid Id { get; }

	public FileMode Mode { get; }

	public FileAccess Access { get; }

	public FileShare Share { get; }

	public Stream AsStream => fileStream;

	public FileProvider(
		string path,
		string? name,
		Guid id,
		FileMode fileMode,
		FileAccess fileAccess,
		FileShare fileShare,
		FileStream fileStream
	)
	{
		Path = path;
		Name = name;
		Id = id;
		Mode = fileMode;
		Access = fileAccess;
		Share = fileShare;
		this.fileStream = fileStream;
	}

	public FileProvider(
		string path,
		string? name,
		Guid id,
		FileMode fileMode,
		FileAccess fileAccess,
		FileShare fileShare
	)
	{
		if (path == null)
			throw new ArgumentNullException(nameof(path));

		if (name == null)
			name = string.Empty;

		Path = path;
		Name = name;
		Id = id;
		Mode = fileMode;
		Access = fileAccess;
		Share = fileShare;

		fileStream = new FileStream(path, fileMode, fileAccess, fileShare);
	}

	public FileProvider(
		string path,
		FileMode fileMode,
		FileAccess fileAccess,
		FileShare fileShare
	) : this(path, string.Empty, Guid.Empty, fileMode, fileAccess, fileShare)
	{
		
	}

	public void Dispose()
	{
		fileStream.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		await fileStream.DisposeAsync();
	}
}