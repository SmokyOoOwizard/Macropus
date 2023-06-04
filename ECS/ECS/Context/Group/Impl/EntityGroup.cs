using Macropus.ECS.Entity;

namespace Macropus.ECS.Context.Group.Impl;

public class EntityGroup : IEntityGroup
{
	private readonly IEnumerable<IEntity> entities;

	public int Count => entities.Count();


	public EntityGroup(IEnumerable<IEntity> entities)
	{
		this.entities = entities;
	}

	public IEnumerable<IEntity> AsEnumerable()
	{
		return entities;
	}
}