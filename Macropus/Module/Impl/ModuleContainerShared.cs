using Macropus.Stuff.Cache;

namespace Macropus.Module.Impl;

// TODO: code gen shared wrappers 
internal class ModuleContainerShared : IModuleContainer
{
    private readonly ICacheRef<IModuleContainer> container;

    public ModuleContainerShared(ICacheRef<IModuleContainer> container)
    {
        this.container = container;
    }

    public void Dispose()
    {
        container.Dispose();
    }
}