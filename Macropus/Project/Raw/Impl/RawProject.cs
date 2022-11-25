using Autofac;
using Macropus.CoolStuff;
using Macropus.FileSystem.Interfaces;
using Macropus.Module;

namespace Macropus.Project.Raw.Impl;

internal class RawProject : IRawProject
{
	private readonly LockFile lockFile;
	private readonly IComponentContext container;

	public IProjectInformationInternal ProjectInformation { get; }
	public IFileSystemService FilesService { get; }
	public IModulesProvider ModuleProvider { get; }

	private bool disposed;

	public RawProject(LockFile lockFile, IComponentContext container)
	{
		this.lockFile = lockFile;
		this.container = container;

		ProjectInformation = container.Resolve<IProjectInformationInternal>();
		FilesService = container.Resolve<IFileSystemService>();
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

	~RawProject()
	{
		Dispose();
	}
}