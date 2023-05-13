using Macropus.ECS;

namespace Odin.ECS;

public interface IECSContexts : IDisposable
{
	IECSContext? GetContext(string contextName);
	IECSContext? CreateContext(string contextName);
	void RemoveContext(string contextName);
}