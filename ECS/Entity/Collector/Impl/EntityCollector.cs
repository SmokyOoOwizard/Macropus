using Macropus.ECS.Component.Filter;

namespace Macropus.ECS.Entity.Collector.Impl;

public class EntityCollector : IEntityCollector
{
	private readonly List<Guid> entities = new();

	public ComponentsFilter Filter { get; }


	public EntityCollector(ComponentsFilter filter)
	{
		Filter = filter;
	}

	public IEnumerable<Guid> GetEntities()
	{
		return entities;
	}

	public void AddEntity(Guid entity)
	{
		entities.Add(entity);
	}

	public void Clear()
	{
		entities.Clear();
	}
}