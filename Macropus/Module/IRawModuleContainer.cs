using Macropus.Interfaces.Module;

namespace Macropus.Module;

public interface IRawModuleContainer : IDisposable
{
	IModule CreateEntryPoint();
}