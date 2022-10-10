using Macropus.FileSystem;
using Macropus.Module;
using Macropus.Stuff.Cache;

namespace Macropus.Project.Provider.Impl;

internal class ProjectProviderShared : IProjectProvider
{
	private readonly ICacheRef<IProjectProvider> cache;

	private bool disposed;

	public ProjectProviderShared(ICacheRef<IProjectProvider> cache)
	{
		this.cache = cache;
	}

	public IProjectInformationInternal ProjectInformation => cache.Value.ProjectInformation;

	public IFileSystemProvider FilesProvider => cache.Value.FilesProvider;

	public IModulesProvider ModuleProvider => cache.Value.ModuleProvider;

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;
		cache.Dispose();
	}

	public ValueTask DisposeAsync()
	{
		if (disposed) return ValueTask.CompletedTask;
		disposed = true;

		cache.Dispose();
		return ValueTask.CompletedTask;
	}
}