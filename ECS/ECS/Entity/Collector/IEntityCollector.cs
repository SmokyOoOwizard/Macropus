using Macropus.ECS.Component.Trigger;

namespace Macropus.ECS.Entity.Collector;

public interface IEntityCollector
{
	ComponentsTrigger Trigger { get; }
	int Count { get; }

	IEnumerable<Guid> GetEntities();
	void AddEntity(Guid entity);
	void Clear();
}