using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Entity.Collector;

namespace Macropus.ECS.Entity.Context;

public abstract class AEntityContext : IEntityContext
{
	private readonly string contextName;

	private readonly IComponentsStorage cold;
	private readonly IComponentsStorage changes;

	private readonly List<IEntityCollector> collectors = new();

	public AEntityContext(
		string contextName,
		IComponentsStorage cold,
		IComponentsStorage changes
	)
	{
		this.contextName = contextName;
		this.cold = cold;
		this.changes = changes;
	}


	public string ContextName() => contextName;

	public IEntityGroup GetGroup(ComponentsFilter filter)
	{
		throw new NotImplementedException();
	}

	public bool HasChanges()
	{
		return changes.EntitiesCount != 0;
	}

	public void AddCollector(IEntityCollector collector)
		=> collectors.Add(collector);

	public void RemoveCollector(IEntityCollector collector)
		=> collectors.Remove(collector);

	public void FillCollectors()
	{
		foreach (var entity in changes.GetEntities())
		{
			// TODO expression tree? maybe it's faster
			foreach (var collector in collectors)
				if (collector.Trigger.Filter(entity, changes))
					collector.AddEntity(entity);
		}
	}

	public void ApplyChanges(IReadOnlyComponentsStorage buffer)
	{
		changes.Apply(buffer);
	}

	public void SaveChanges()
	{
		cold.Apply(changes);
		changes.Clear();
	}
}