using Macropus.Interfaces.Module;

namespace Macropus.Module;

internal struct LoadedModule : IDisposable
{
	public IRawModuleContainer RawModuleContainer { get; set; }
	public IModule Module { get; set; }
	public ModuleBuilder Bindings { get; set; }

	public void Dispose()
	{
		Module?.Dispose();
		RawModuleContainer?.Dispose();
	}
}