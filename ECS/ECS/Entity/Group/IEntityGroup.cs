namespace Macropus.ECS.Entity.Context.Group;

public interface IEntityGroup
{
	int Count { get; }

	IEnumerable<IEntity> AsEnumerable();
}