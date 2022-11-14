using Macropus.ECS.Component.Storage;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Entity;
using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;
using Macropus.ECS.Systems.Extensions;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly ASystem[] systems;
	private readonly Dictionary<ASystem, ReactiveSystemContext> reactiveSystems = new();

	private readonly IComponentsStorage changes = new ComponentsStorage();

	public SystemsExecutor(params ASystem[] systems)
	{
		foreach (var system in systems)
		{
			var filter = system.GetFilter();
			if (filter != null)
			{
				reactiveSystems[system] = new(filter.Value);
			}
		}

		this.systems = systems;
	}

	public void SetCollectors(EntitiesContext context)
	{
		foreach (var system in reactiveSystems)
		{
			context.AddCollector(system.Value.GetCollector());
		}
	}

	public void RemoveCollectors(EntitiesContext context)
	{
		foreach (var system in reactiveSystems)
		{
			context.RemoveCollector(system.Value.GetCollector());
		}
	}

	public void Execute(EntitiesContext context)
	{
		if (!context.HasChanges())
			return;

		foreach (var system in systems)
		{
			(system as IUpdateSystem)?.Update();

			if (reactiveSystems.TryGetValue(system, out var systemContext))
			{
				var collector = systemContext.GetCollector();
				systemContext.SwapCollector(context);

				(system as IReactiveSystem)?.Execute(EntityWrapper.Wrap(collector.GetEntities(),
					context.GetHotComponentsStorage(), changes));
				collector.Clear();

				context.ApplyBuffer(changes);
				changes.Clear();
			}
		}
	}
}