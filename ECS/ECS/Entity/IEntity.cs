using System;
using Macropus.ECS.Component;

namespace Macropus.ECS.Entity;

public interface IEntity
{
	Guid Id { get; }

	bool HasComponent<T>() where T : struct, IComponent;
	T GetComponent<T>() where T : struct, IComponent;
	void ReplaceComponent<T>(T component) where T : struct, IComponent;
	void RemoveComponent<T>() where T : struct, IComponent;
}