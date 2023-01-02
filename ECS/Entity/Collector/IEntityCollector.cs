using Macropus.ECS.Component.Filter;

namespace Macropus.ECS.Entity.Collector;

public interface IEntityCollector
{
	ComponentsFilter Filter { get; }
	int Count { get; }

	IEnumerable<Guid> GetEntities();
	void AddEntity(Guid entity);
	void Clear();
}