using Macropus.ECS.Entity.Context;

namespace Odin.ECS;

public interface IECSContexts : IDisposable, IAsyncDisposable
{
	IEntityContext? GetContext(string contextName);
}