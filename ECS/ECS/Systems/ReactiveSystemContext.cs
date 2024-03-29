﻿using Macropus.ECS.Component.Trigger;
using Macropus.ECS.Context.Impl;
using Macropus.ECS.Entity.Collector;
using Macropus.ECS.Entity.Collector.Impl;

namespace Macropus.ECS.Systems;

public sealed class ReactiveSystemContext
{
	private IEntityCollector coldCollector;
	private IEntityCollector hotCollector;

	public ReactiveSystemContext(ComponentsTrigger trigger)
	{
		coldCollector = new EntityCollector(trigger);
		hotCollector = new EntityCollector(trigger);
	}

	public IEntityCollector GetCollector()
		=> hotCollector;

	public void SwapCollector(EntityContext context)
	{
		var tmp = hotCollector;

		context.RemoveCollector(tmp);
		context.AddCollector(coldCollector);

		hotCollector = coldCollector;
		coldCollector = tmp;
	}
}