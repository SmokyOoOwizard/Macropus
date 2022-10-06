using Macropus.FileSystem;
using Macropus.Module;
using Macropus.Project.Instance;

namespace Macropus.Project.Provider;

internal interface IProjectProvider : IDisposable, IAsyncDisposable
{
    IProjectInformationInternal ProjectInformation { get; }

    IFileSystemProvider FilesProvider { get; }

    IModulesProvider ModuleProvider { get; }
    // raw access to project data/settings/db
}