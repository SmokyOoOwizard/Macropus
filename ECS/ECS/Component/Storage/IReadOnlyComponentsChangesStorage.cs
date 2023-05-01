using System;

namespace Macropus.ECS.Component.Storage;

public interface IReadOnlyComponentsChangesStorage : IReadOnlyComponentsStorage
{
	bool HadComponent(Guid entityId, string name);
	bool HadComponent<T>(Guid entityId) where T : struct, IComponent;
}