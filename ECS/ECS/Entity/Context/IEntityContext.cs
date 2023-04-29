using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Entity.Collector;
using Macropus.ECS.Entity.Context.Group;

namespace Macropus.ECS.Entity.Context;

public interface IEntityContext
{
	string ContextName { get; }
	IEntityGroup GetGroup(ComponentsFilter filter);

	bool HasChanges();
	IComponentsStorage GetChanges();

	void AddCollector(IEntityCollector collector);
	void RemoveCollector(IEntityCollector collector);
}