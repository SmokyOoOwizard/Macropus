using Macropus.ECS;
using Macropus.ECS.Entity.Context;

namespace Odin.ECS.Impl;

internal class EcsContexts : IECSContextsInternal
{
	private readonly Dictionary<EntityContext, SystemsExecutor> contexts = new();

	public IEntityContext? GetContext(string contextName)
	{
		return contexts.Keys.FirstOrDefault(context => context.ContextName == contextName);
	}

	public async Task TickAsync()
	{
		var executorsTasks = contexts.Values.Select(executor => Task.Run(executor.Execute)).ToArray();

		await Task.WhenAll(executorsTasks);
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public ValueTask DisposeAsync()
	{
		throw new NotImplementedException();
	}
}