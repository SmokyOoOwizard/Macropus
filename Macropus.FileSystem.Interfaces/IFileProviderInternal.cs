namespace Macropus.FileSystem.Interfaces;

public interface IFileProviderInternal : IFileProvider
{
	string Path { get; }
}