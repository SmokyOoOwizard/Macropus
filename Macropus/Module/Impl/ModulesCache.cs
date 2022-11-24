using Macropus.Extensions;
using Macropus.FileSystem;
using Macropus.FileSystem.Extensions;
using Macropus.FileSystem.Interfaces;
using Macropus.Stuff;
using Macropus.Stuff.Cache;

namespace Macropus.Module.Impl;

internal class ModulesCache : IModulesCache
{
	private readonly CacheWithRefs<IRawModuleContainer, string> cache = new();

	private bool disposed;

	public async Task<ICacheRef<IRawModuleContainer, string>> GetOrLoadModuleAsync(
		IFileProvider file,
		CancellationToken cancellationToken
	)
	{
		if (disposed) throw new ObjectDisposedException(nameof(ModulesCache));

		var fileStream = file.AsStream;
		var moduleHash = await file.GetFileHashAsync(cancellationToken);

		lock (cache)
		{
			if (!cache.ContainsKey(moduleHash))
			{
				var assemblyContext = new CollectibleAssemblyLoadContext();
				var loadedAssembly = assemblyContext.LoadFromStream(fileStream);

				var moduleContainer = new RawModuleContainer(loadedAssembly, assemblyContext);

				return cache.GetOrAdd(moduleHash, moduleContainer);
			}

			return cache.Get(moduleHash);
		}
	}

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		lock (cache)
		{
			cache.Dispose();
		}
	}
}