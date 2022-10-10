﻿using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage;

public interface IReadOnlyComponentsStorage
{
	uint ComponentsCount { get; }
	uint EntitiesCount { get; }

	bool HasComponent<T>(Guid entityId) where T : struct, IComponent;
	bool HasComponent(Guid entityId, string name);
	T GetComponent<T>(Guid entityId) where T : struct, IComponent;
	IComponent GetComponent(Guid entityId, string name);

	IEnumerable<Guid> GetEntities();
}