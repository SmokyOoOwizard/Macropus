using Macropus.ECS.Component.Filter;
using Macropus.ECS.Entity.Collector;

namespace Macropus.ECS.Entity.Context;

public interface IEntityContext
{
	string ContextName { get; }
	IEntityGroup GetGroup(ComponentsFilter filter);

	bool HasChanges();

	void AddCollector(IEntityCollector collector);
	void RemoveCollector(IEntityCollector collector);
}