using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Entity.Collector;

namespace Macropus.ECS.Entity.Context;

public class EntityContext : IEntityContext
{
	public string ContextName { get; }

	public readonly IComponentsStorage cold;
	public readonly IComponentsStorage changes;

	private readonly List<IEntityCollector> collectors = new();
	private readonly MergedComponentsStorage applyBufferMergedComponentsChanges = new();

	public EntityContext(
		string contextName,
		IComponentsStorage cold,
		IComponentsStorage changes
	)
	{
		ContextName = contextName;
		this.cold = cold;
		this.changes = changes;
	}

	public EntityContext(
		string contextName,
		IComponentsStorage cold
	)
	{
		ContextName = contextName;
		this.cold = cold;
		changes = new ComponentsStorage();
	}

	public IEntityGroup GetGroup(ComponentsFilter filter)
	{
		var entities = EntityWrapper.Wrap(cold.GetEntities(filter), cold, changes);
		return new EntityGroup(entities);
	}

	public bool HasChanges()
		=> changes.EntitiesCount != 0;

	public void AddCollector(IEntityCollector collector)
		=> collectors.Add(collector);

	public void RemoveCollector(IEntityCollector collector)
		=> collectors.Remove(collector);

	public void ApplyChanges(IReadOnlyComponentsStorage buffer)
	{
		applyBufferMergedComponentsChanges.SetStorages(buffer, changes);

		foreach (var entity in buffer.GetEntities())
		{
			// TODO expression tree? maybe it's faster
			foreach (var collector in collectors)
				if (collector.Trigger.Filter(entity, applyBufferMergedComponentsChanges))
					collector.AddEntity(entity);
		}

		changes.Apply(buffer);
	}

	public void SaveChanges()
	{
		cold.Apply(changes);
		changes.Clear();
	}
}