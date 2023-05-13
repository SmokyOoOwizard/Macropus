using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly EntityContext context;

	private readonly ISystem[] systems;
	private readonly Dictionary<ISystem, ReactiveSystemContext> reactiveSystems = new();

	public SystemsExecutor(EntityContext context, IEnumerable<ISystem> systems)
	{
		this.context = context;
		this.systems = systems.ToArray();

		foreach (var system in this.systems)
		{
			if (system is IReactiveSystem reactiveSystem)
			{
				var trigger = reactiveSystem.GetTrigger();

				var reactive = new ReactiveSystemContext(trigger);
				reactiveSystems[system] = reactive;

				context.AddCollector(reactive.GetCollector());
			}
		}
	}

	public void Execute()
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