using ECS.Tests.Filter.Components;
using Macropus.ECS.Component.Filter;
using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;

namespace ECS.Tests.Remove.Systems;

public class RemoveTestComponentSystem : ASystem, IUpdateSystem
{
	private readonly IEntityContext context;

	public RemoveTestComponentSystem(IEntityContext context)
	{
		this.context = context;
	}

	private ComponentsFilter GetFilter()
	{
		return ComponentsFilter.Empty;
	}

	public void Update()
	{
		var filter = GetFilter();
		var entities = context.GetGroup(filter);

		Assert.NotEmpty(entities);

		foreach (var entity in entities)
			entity.RemoveComponent<EmptyTestComponent1>();
	}
}