using ECS.Tests.Trigger.Components;
using Macropus.ECS.Component.Trigger;
using Macropus.ECS.Entity;
using Macropus.ECS.Systems;

namespace ECS.Tests.Trigger.Systems;

public class TriggerWithSubTriggerSystem : IReactiveSystem
{
	public ComponentsTrigger GetTrigger()
	{
		return ComponentsTrigger.AnyOf(
				ComponentsTrigger.AllOf(typeof(EmptyTestComponent1), typeof(EmptyTestComponent2)),
				ComponentsTrigger.AllOf(typeof(EmptyTestComponent1))
					.NoneOf(typeof(EmptyTestComponent3)))
			.Build();
	}

	public void Execute(IEnumerable<IEntity> entities)
	{
		Assert.NotEmpty(entities);

		foreach (var entity in entities)
			entity.RemoveComponent<EmptyTestComponent4>();
	}
}