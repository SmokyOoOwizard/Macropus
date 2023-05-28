using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Context.Group;
using Macropus.ECS.Entity.Collector;

namespace Macropus.ECS.Context;

public interface IEntityContext : IDisposable
{
	string ContextName { get; }
	IEntityGroup GetGroup(ComponentsFilter filter);

	bool HasChanges();
	IComponentsStorage GetChanges();

	void AddCollector(IEntityCollector collector);
	void RemoveCollector(IEntityCollector collector);
}