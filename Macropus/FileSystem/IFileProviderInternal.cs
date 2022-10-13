namespace Macropus.FileSystem;

public interface IFileProviderInternal : IFileProvider
{
	string Path { get; }
}