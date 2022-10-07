using Macropus.Module.Impl;
using Macropus.Stuff.Cache;

namespace Macropus.Module.Extensions;

internal static class ModuleContainerExtensions
{
	public static IRawModuleContainer ToSharedModule(this ICacheRef<IRawModuleContainer> cacheRef)
	{
		return new RawModuleContainerShared(cacheRef);
	}
}