using Macropus.FileSystem.Interfaces;

namespace Macropus.Database.Interfaces;

public interface IDatabasesServiceFactory
{
	Task<IDatabasesService> Create(
		string path,
		IFileSystemService fileSystemService,
		CancellationToken cancellationToken = default
	);
}