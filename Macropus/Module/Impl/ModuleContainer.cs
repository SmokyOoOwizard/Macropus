using System.Reflection;
using Macropus.Stuff;

namespace Macropus.Module.Impl;

internal class ModuleContainer : IModuleContainer
{
    private readonly Assembly loadedAssembly;
    private readonly CollectibleAssemblyLoadContext assemblyContext;

    private bool disposed;

    public ModuleContainer(Assembly loadedAssembly, CollectibleAssemblyLoadContext assemblyContext)
    {
        this.loadedAssembly = loadedAssembly;
        this.assemblyContext = assemblyContext;
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;

        assemblyContext.Unload();
    }
}