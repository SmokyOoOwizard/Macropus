using ECS.Tests.Trigger.Components;
using Macropus.ECS.Component.Trigger;
using Macropus.ECS.Entity;
using Macropus.ECS.Systems;

namespace ECS.Tests.Trigger.Systems;

public class FewAllOfTriggerSystem : IReactiveSystem
{
	public ComponentsTrigger GetTrigger()
	{
		return ComponentsTrigger.AllOf(typeof(EmptyTestComponent1), typeof(EmptyTestComponent2)).Build();
	}

	public void Execute(IEnumerable<IEntity> entities)
	{
		Assert.NotEmpty(entities);

		foreach (var entity in entities)
			entity.RemoveComponent<EmptyTestComponent3>();
	}
}