using Macropus.Module.Impl;
using Macropus.Stuff.Cache;

namespace Macropus.Module.Extensions;

internal static class ModuleContainerExtensions
{
    public static IModuleContainer ToSharedModule(this ICacheRef<IModuleContainer> cacheRef)
    {
        return new ModuleContainerShared(cacheRef);
    }
}