using Macropus.ECS;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Entity.Context;
using Macropus.ECS.Impl;

namespace Odin.ECS.Impl;

internal class EcsContexts : IECSContextsInternal
{
	private readonly List<ECSContext> contexts = new();

	public IECSContext? GetContext(string contextName)
	{
		return contexts.FirstOrDefault(context => context.EntityContext.ContextName == contextName);
	}

	public IECSContext? CreateContext(string contextName)
	{
		if (contexts.Any(context => context.EntityContext.ContextName == contextName))
			return null;

		// TODO replace in memory storage
		var entityContext = new EntityContext(contextName, new ComponentsStorageInMemory());

		var context = new ECSContext(entityContext);

		contexts.Add(context);

		return context;
	}

	public void RemoveContext(string contextName)
	{
		var context = contexts.FirstOrDefault(context => context.EntityContext.ContextName == contextName);
		if(context == null)
			return;

		contexts.Remove(context);

		context.EntityContext.Dispose();
	}

	public async Task TickAsync()
	{
		// TODO rewrite executor. what if system change entity in other context?
		var executorsTasks = contexts.Select(executor => Task.Run(executor.Tick)).ToArray();

		await Task.WhenAll(executorsTasks);
	}

	public void Dispose()
	{
		foreach (var context in contexts)
		{
			context.EntityContext.Dispose();
		}
	}
}