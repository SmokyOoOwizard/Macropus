using Macropus.FileSystem.Interfaces;
using Macropus.Module;

namespace Macropus.Project.Raw;

public interface IRawProject : IDisposable, IAsyncDisposable
{
	IProjectInformationInternal ProjectInformation { get; }

	IFileSystemProvider FilesProvider { get; }

	IModulesProvider ModuleProvider { get; }
	// raw access to project data/settings/db
}