using System.Data.Common;

namespace Macropus.FileSystem.Interfaces;

public interface IFileSystemServiceFactory
{
	Task<IFileSystemService> Create(
		string objLocation,
		DbConnection dbConnection,
		CancellationToken cancellationToken = default
	);

	Task<IFileSystemService> Create(
		string objLocation,
		string dbPath,
		CancellationToken cancellationToken = default
	);
}