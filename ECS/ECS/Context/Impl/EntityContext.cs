using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Context.Group;
using Macropus.ECS.Context.Group.Impl;
using Macropus.ECS.Entity;
using Macropus.ECS.Entity.Collector;

namespace Macropus.ECS.Context.Impl;

public class EntityContext : IEntityContext
{
	public string ContextName { get; }

	private readonly IComponentsStorage cold;
	private readonly IComponentsChangesStorage changes;

	private readonly List<IEntityCollector> collectors = new();

	public EntityContext(string contextName, IComponentsStorage cold, IComponentsChangesStorage changes)
	{
		ContextName = contextName;
		this.cold = cold;
		this.changes = changes;
	}

	public EntityContext(string contextName, IComponentsStorage cold) : this(contextName, cold, new ComponentsChangesStorageInMemory()) { }

	public IEntityGroup GetGroup(ComponentsFilter filter)
	{
		var entities = EntityWrapper.Wrap(cold.GetEntities(filter), cold, changes);

		return new EntityGroup(entities);
	}

	public IEntityGroup GetGroup(IEntityCollector collector)
	{
		var entities = EntityWrapper.Wrap(collector.GetEntities(), cold, changes);

		return new EntityGroup(entities);
	}

	public bool HasChanges()
		=> changes.EntitiesCount != 0;

	public IComponentsStorage GetChanges()
		=> changes;

	public void AddCollector(IEntityCollector collector)
		=> collectors.Add(collector);

	public void RemoveCollector(IEntityCollector collector)
		=> collectors.Remove(collector);

	public void ApplyChanges(IReadOnlyComponentsStorage buffer)
	{
		changes.Apply(buffer);
	}

	public void SaveChanges()
	{
		foreach (var entity in changes.GetEntities())
		{
			// TODO expression tree? maybe it's faster
			foreach (var collector in collectors)
				if (collector.Trigger.Filter(entity, cold, changes))
					collector.AddEntity(entity);
		}

		cold.Apply(changes);
		changes.Clear();
	}

	public void Dispose()
	{
		cold.Dispose();
		changes.Dispose();
	}
}