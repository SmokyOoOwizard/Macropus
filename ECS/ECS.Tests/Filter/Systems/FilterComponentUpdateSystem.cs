using ECS.Tests.Filter.Components;
using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Filter.Builder;
using Macropus.ECS.Context;
using Macropus.ECS.Systems;

namespace ECS.Tests.Filter.Systems;

public class FilterComponentUpdateSystem : ISystem, IUpdateSystem
{
	private readonly IEntityContext context;

	public FilterComponentUpdateSystem(IEntityContext context)
	{
		this.context = context;
	}

	private ComponentsFilter GetFilter()
	{
		return ComponentsFilter.AnyOf(
				ComponentsFilter.AllOf(typeof(EmptyTestComponent2), typeof(EmptyTestComponent4), typeof(EmptyTestComponent7)),
				ComponentsFilter.AllOf(typeof(EmptyTestComponent10)).NoneOf(typeof(EmptyTestComponent6)))
			.Build();
	}

	public void Update()
	{
		var filter = GetFilter();
		var entities = context.GetGroup(filter);

		Assert.NotEmpty(entities.AsEnumerable());

		foreach (var entity in entities.AsEnumerable())
		{
			if (entity.HasComponent<EmptyTestComponent2>()
			    && entity.HasComponent<EmptyTestComponent4>()
			    && entity.HasComponent<EmptyTestComponent7>())
				return;

			if (entity.HasComponent<EmptyTestComponent10>() && !entity.HasComponent<EmptyTestComponent6>())
				return;

			throw new Exception();
		}
	}
}