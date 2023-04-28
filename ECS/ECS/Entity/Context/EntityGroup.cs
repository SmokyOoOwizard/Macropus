using System.Collections;

namespace Macropus.ECS.Entity.Context;

public class EntityGroup : IEntityGroup
{
	private readonly IEnumerable<IEntity> entities;

	public EntityGroup(IEnumerable<IEntity> entities)
	{
		this.entities = entities;
	}

	public IEnumerator<IEntity> GetEnumerator()
	{
		return entities.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return entities.GetEnumerator();
	}
}