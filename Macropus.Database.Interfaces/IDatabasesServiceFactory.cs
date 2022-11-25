using Macropus.FileSystem.Interfaces;

namespace Macropus.Database.Interfaces;

public interface IDatabasesServiceFactory
{
	Task<IDatabasesService> Create(
		string path,
		IFileSystemProvider fileSystemProvider,
		CancellationToken cancellationToken = default
	);
}