using Macropus.Interfaces.Module;
using Macropus.Stuff.Cache;

namespace Macropus.Module.Impl;

// TODO: code gen shared wrappers 
internal class RawModuleContainerShared : IRawModuleContainer
{
	private readonly ICacheRef<IRawModuleContainer> container;

	public RawModuleContainerShared(ICacheRef<IRawModuleContainer> container)
	{
		this.container = container;
	}

	public IModule CreateEntryPoint()
	{
		return container.Value.CreateEntryPoint();
	}

	public void Dispose()
	{
		container.Dispose();
	}
}