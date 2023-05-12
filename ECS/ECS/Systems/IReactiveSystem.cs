using System.Collections.Generic;
using Macropus.ECS.Component.Trigger;
using Macropus.ECS.Entity;

namespace Macropus.ECS.Systems;

public interface IReactiveSystem : ISystem
{
	ComponentsTrigger GetTrigger();
	void Execute(IEnumerable<IEntity> entities);
}