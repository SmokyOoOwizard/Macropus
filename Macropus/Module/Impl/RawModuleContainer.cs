using System.Reflection;
using Macropus.Interfaces.Module;
using Macropus.Stuff;

namespace Macropus.Module.Impl;

internal class RawModuleContainer : IRawModuleContainer
{
	private readonly Assembly loadedAssembly;
	private readonly CollectibleAssemblyLoadContext assemblyContext;

	private bool disposed;

	public RawModuleContainer(Assembly loadedAssembly, CollectibleAssemblyLoadContext assemblyContext)
	{
		this.loadedAssembly = loadedAssembly;
		this.assemblyContext = assemblyContext;
	}

	public IModule CreateEntryPoint()
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		assemblyContext.Unload();
	}
}