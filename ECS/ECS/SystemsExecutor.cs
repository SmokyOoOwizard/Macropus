using System.Collections.Generic;
using System.Linq;
using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;
using Macropus.ECS.Systems.Extensions;

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly ISystem[] systems;
	private readonly Dictionary<ISystem, ReactiveSystemContext> reactiveSystems = new();

	public SystemsExecutor(IEnumerable<ISystem> systems)
	{
		this.systems = systems.ToArray();
		foreach (var system in this.systems)
		{
			var trigger = system.GetTrigger();
			if (trigger != null)
			{
				reactiveSystems[system] = new(trigger.Value);
			}
		}
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
			switch (system)
			{
				case IUpdateSystem updateSystem:
					updateSystem.Update();
					break;
				case IReactiveSystem reactiveSystem when reactiveSystems.TryGetValue(system, out var systemContext):
				{
					var collector = systemContext.GetCollector();
					if (collector.Count == 0)
						break;

					systemContext.SwapCollector(context);

					var entities = context.GetGroup(collector);
					reactiveSystem.Execute(entities.AsEnumerable());

					collector.Clear();
					break;
				}
			}

			context.SaveChanges();
		}
	}
}