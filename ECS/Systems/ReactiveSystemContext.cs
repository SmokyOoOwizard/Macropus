using Macropus.ECS.Component.Filter;
using Macropus.ECS.Entity.Collector;
using Macropus.ECS.Entity.Collector.Impl;
using Macropus.ECS.Entity.Context;

namespace Macropus.ECS.Systems;

public sealed class ReactiveSystemContext
{
	private IEntityCollector coldCollector;
	private IEntityCollector hotCollector;

	public ReactiveSystemContext(ComponentsFilter filter)
	{
		coldCollector = new EntityCollector(filter);
		hotCollector = new EntityCollector(filter);
	}

	public IEntityCollector GetCollector()
		=> hotCollector;

	public void SwapCollector(EntitiesContext context)
	{
		var tmp = hotCollector;

		context.RemoveCollector(tmp);
		context.AddCollector(coldCollector);

		hotCollector = coldCollector;
		coldCollector = tmp;
	}
}