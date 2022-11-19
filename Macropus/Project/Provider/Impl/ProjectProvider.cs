using Autofac;
using Macropus.CoolStuff;
using Macropus.FileSystem;
using Macropus.Module;

namespace Macropus.Project.Provider.Impl;

internal class ProjectProvider : IProjectProvider
{
	private readonly LockFile lockFile;
	private readonly IComponentContext container;

	public IProjectInformationInternal ProjectInformation { get; }
	public IFileSystemProvider FilesProvider { get; }
	public IModulesProvider ModuleProvider { get; }

	private bool disposed;

	public ProjectProvider(LockFile lockFile, IComponentContext container)
	{
		this.lockFile = lockFile;
		this.container = container;

		ProjectInformation = container.Resolve<IProjectInformationInternal>();
		FilesProvider = container.Resolve<IFileSystemProvider>();
		ModuleProvider = container.Resolve<IModulesProvider>();
	}

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		lockFile.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		if (disposed) return;
		disposed = true;

		await lockFile.DisposeAsync();
	}

	~ProjectProvider()
	{
		Dispose();
	}
}