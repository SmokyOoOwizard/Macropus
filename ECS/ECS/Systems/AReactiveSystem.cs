using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Trigger;
using Macropus.ECS.Entity;
using Macropus.ECS.Entity.Context;

namespace Macropus.ECS.Systems;

public abstract class AReactiveSystem : ASystem
{
	private readonly IEntityContext context;

	public AReactiveSystem(IEntityContext context)
	{
		this.context = context;
	}

	public abstract ComponentsTrigger GetTrigger();
	public abstract ComponentsFilter GetFilter();
	public abstract void Execute(IEnumerable<IEntity> entities);
}