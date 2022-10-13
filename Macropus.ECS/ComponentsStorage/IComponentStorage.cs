﻿using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage;

public interface IComponentStorage<T> : IComponentStorage where T : struct, IComponent
{
	T? GetGenericComponent(Guid entity);
	void ReplaceComponent(Guid entity, T component);
}

public interface IComponentStorage
{
	string ComponentName { get; }

	bool HasEntity(Guid entity);
	IComponent? GetComponent(Guid entity);
	void RemoveComponent(Guid entity);

	void Clear();

	IReadOnlyCollection<Guid> GetEntities();
}