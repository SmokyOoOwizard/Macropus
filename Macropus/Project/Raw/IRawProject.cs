using Macropus.Database.Interfaces;
using Macropus.FileSystem.Interfaces;

namespace Macropus.Project.Raw;

public interface IRawProject : IDisposable, IAsyncDisposable
{
	IProjectInformationInternal ProjectInformation { get; }

	IFileSystemService FilesService { get; }
	IDatabasesService DatabasesService { get; }

	// raw access to project data/settings/db
}