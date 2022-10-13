using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage;

public interface IComponentTypesStorage
{
	uint GetComponentTypeId<T>() where T : struct, IComponent;
	uint GetComponentTypeId(string name);
}