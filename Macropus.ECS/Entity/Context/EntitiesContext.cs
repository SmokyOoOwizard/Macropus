using Macropus.ECS.Component.Storage;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Entity.Collector;

namespace Macropus.ECS.Entity.Context;

public sealed class EntitiesContext
{
	private readonly IComponentsStorage componentsStorage;
	private readonly ComponentsStorage changesComponent = new();
	private readonly List<IEntityCollector> collectors = new();

	private readonly MergedComponentsStorage applyBufferMergedComponentsChanges = new();
	
	private readonly MergedComponentsStorage mergedHotComponentsStorage;


	public EntitiesContext(IComponentsStorage componentsStorage)
	{
		this.componentsStorage = componentsStorage;

		mergedHotComponentsStorage = new(changesComponent, componentsStorage);
	}

	public int CollectorsCount => collectors.Count;

	public IReadOnlyComponentsStorage GetColdComponentsStorage()
		=> componentsStorage;
	
	public IReadOnlyComponentsStorage GetHotComponentsStorage()
		=> mergedHotComponentsStorage;

	public void SaveChanges()
	{
		componentsStorage.Apply(changesComponent);
		changesComponent.Clear();
	}

	public bool HasChanges()
		=> changesComponent.EntitiesCount != 0;

	public void AddCollector(IEntityCollector collector)
	{
		collectors.Add(collector);
	}

	public void RemoveCollector(IEntityCollector collector)
	{
		collectors.Remove(collector);
	}

	public void ApplyBuffer(IReadOnlyComponentsStorage buffer)
	{
		applyBufferMergedComponentsChanges.SetStorages(buffer, changesComponent);

		foreach (var entity in buffer.GetEntities())
		{
			// TODO expression tree? maybe it's faster 
			foreach (var collector in collectors)
				if (collector.Filter.Filter(entity, applyBufferMergedComponentsChanges))
					collector.AddEntity(entity);
		}
		
		changesComponent.Apply(buffer);
	}
}