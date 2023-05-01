using System.Collections.Generic;

namespace Macropus.ECS.Entity.Group;

public interface IEntityGroup
{
	int Count { get; }

	IEnumerable<IEntity> AsEnumerable();
}