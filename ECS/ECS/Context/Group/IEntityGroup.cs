using Macropus.ECS.Entity;

namespace Macropus.ECS.Context.Group;

public interface IEntityGroup
{
	int Count { get; }

	IEnumerable<IEntity> AsEnumerable();
}