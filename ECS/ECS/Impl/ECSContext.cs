using Macropus.ECS.Context;
using Macropus.ECS.Context.Impl;
using Macropus.ECS.Systems;

namespace Macropus.ECS.Impl;

public sealed class ECSContext : IECSContext
{
	public IEntityContext EntityContext => context;

	private readonly EntityContext context;

	private readonly List<ISystem> systems = new();
	private readonly Dictionary<ISystem, ReactiveSystemContext> reactiveSystems = new();

	public ECSContext(EntityContext context)
	{
		this.context = context;
	}

	public ECSContext(EntityContext context, IEnumerable<ISystem> systems) : this(context)
	{
		foreach (var system in systems)
		{
			AddSystem(system);
		}
	}

	public void AddSystem(ISystem system)
	{
		systems.Add(system);
		if (system is IReactiveSystem reactiveSystem)
		{
			var trigger = reactiveSystem.GetTrigger();

			var reactive = new ReactiveSystemContext(trigger);
			reactiveSystems[system] = reactive;

			context.AddCollector(reactive.GetCollector());
		}
	}

	public void RemoveSystem(ISystem system)
	{
		systems.Remove(system);
		if (reactiveSystems.TryGetValue(system, out var reactive))
		{
			reactiveSystems.Remove(system);

			context.RemoveCollector(reactive.GetCollector());
		}
	}

	public void Tick()
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