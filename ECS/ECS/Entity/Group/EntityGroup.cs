using System.Collections.Generic;
using System.Linq;

namespace Macropus.ECS.Entity.Group;

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