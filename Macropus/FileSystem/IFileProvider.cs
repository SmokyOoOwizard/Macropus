namespace Macropus.FileSystem;

public interface IFileProvider : IDisposable, IAsyncDisposable
{
	string? Name { get; }
	Guid Id { get; }

	FileMode Mode { get; }
	FileAccess Access { get; }
	FileShare Share { get; }


	Stream AsStream { get; }
}