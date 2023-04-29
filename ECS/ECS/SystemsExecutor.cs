using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;
using Macropus.ECS.Systems.Extensions;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly ISystem[] systems;
	private readonly Dictionary<ISystem, ReactiveSystemContext> reactiveSystems = new();

	public SystemsExecutor(params ISystem[] systems)
	{
		foreach (var system in systems)
		{
			var trigger = system.GetTrigger();
			if (trigger != null)
			{
				reactiveSystems[system] = new(trigger.Value);
			}
		}

		this.systems = systems;
	}

	public void SetContext(EntityContext context)
	{
		foreach (var (_, rContext) in reactiveSystems)
		{
			context.AddCollector(rContext.GetCollector());
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

				if (system is IReactiveSystem reactiveSystem)
				{
					var entities = context.GetGroup(collector);
					reactiveSystem.Execute(entities.AsEnumerable());
				}

				collector.Clear();
				context.SaveChanges();
			}
		}
	}
}