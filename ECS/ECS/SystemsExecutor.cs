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
			var filter = system.GetTrigger();
			if (filter != null)
			{
				reactiveSystems[system] = new(filter.Value);
			}
		}

		this.systems = systems;
	}

	public void SetCollectors(EntityContext context)
	{
		foreach (var system in reactiveSystems)
		{
			context.AddCollector(system.Value.GetCollector());
		}
	}

	public void RemoveCollectors(EntityContext context)
	{
		foreach (var system in reactiveSystems)
		{
			context.RemoveCollector(system.Value.GetCollector());
		}
	}

	public void Execute(EntityContext context)
	{
		foreach (var system in systems)
		{
			(system as IUpdateSystem)?.Update();

			if (reactiveSystems.TryGetValue(system, out var systemContext))
			{
				var collector = systemContext.GetCollector();
				if (collector.Count == 0)
					continue;

				systemContext.SwapCollector(context);

				if (system is AReactiveSystem reactiveSystem)
				{
					var entities = EntityWrapper.Wrap(collector.GetEntities(), context.cold, changes);

					reactiveSystem.Execute(entities);
				}

				collector.Clear();
				context.ApplyChanges(changes);
				context.SaveChanges();
				changes.Clear();
			}
		}
	}
}