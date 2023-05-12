using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Macropus.ECS.Component.Exceptions;
using Macropus.ECS.Component.Filter;
using Macropus.Linq;

namespace Macropus.ECS.Component.Storage.Impl;

public class ComponentsStorage : IComponentsStorage
{
	private readonly Dictionary<string, ComponentStorage> storage = new();
	private readonly HashSet<Guid> existsEntities = new();

	public uint ComponentsCount => (uint)storage.Count;
	public uint EntitiesCount => (uint)existsEntities.Count;

	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		if (!TryGetStorage<T>(out var entities))
			return false;

		if (!entities.GetGenericComponent(entityId, out _))
			return false;

		return true;
	}

	public bool HasComponent(Guid entityId, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return false;

		if (!TryGetStorage(name, out var entities))
			return false;

		var component = entities.GetComponent(entityId);

		return component != null;
	}

	public T GetComponent<T>(Guid entityId) where T : struct, IComponent
	{
		if (!TryGetStorage<T>(out var entities))
			throw new ComponentNotFoundException();

		if (!entities.GetGenericComponent(entityId, out var component))
			throw new ComponentNotFoundException();

		return component;
	}

	public IComponent GetComponent(Guid entityId, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentNullException(nameof(name));

		if (!TryGetStorage(name, out var entities))
			throw new ComponentNotFoundException();

		var component = entities.GetComponent(entityId);

		if (component == null)
			throw new ComponentNotFoundException();

		return component;
	}


	public void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent
	{
		if (!TryGetStorage<T>(out var entities))
			AddStorage(out entities);

		entities.ReplaceComponent(entityId, component);

		existsEntities.Add(entityId);
	}

	public void RemoveComponent<T>(Guid entityId) where T : struct, IComponent
	{
		if (!TryGetStorage<T>(out var entities))
		{
			AddStorage(out entities);
		}

		entities.RemoveComponent(entityId);

		existsEntities.Add(entityId);
	}

	public void Apply(IReadOnlyComponentsStorage changes)
	{
		foreach (var components in changes.GetComponents())
		{
			ComponentStorage st;
			if (TryGetStorage(components.ComponentName, out st))
				st.Apply(components);
			else
			{
				AddStorage(components.ComponentType, out st);
				st.Apply(components);
			}
		}

		changes.GetEntities().Fill(existsEntities);
	}

	public IEnumerable<Guid> GetEntities()
	{
		return existsEntities;
	}

	public IEnumerable<Guid> GetEntities(ComponentsFilter filter)
	{
		foreach (var entity in existsEntities)
		{
			if (filter.Filter(entity, this))
				yield return entity;
		}
	}

	public IEnumerable<IReadOnlyComponentStorage> GetComponents()
	{
		return storage.Values;
	}

	public void Clear()
	{
		foreach (var componentType in storage)
			componentType.Value.Clear();

		existsEntities.Clear();
	}

	private bool TryGetStorage<T>(out ComponentStorage<T> componentStorage) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (!storage.TryGetValue(componentName, out var cStorage))
		{
			componentStorage = null!;
			return false;
		}

		componentStorage = (cStorage as ComponentStorage<T>)!;
		return true;
	}

	private bool TryGetStorage(string componentName, out ComponentStorage componentStorage)
	{
		return storage.TryGetValue(componentName, out componentStorage!);
	}

	private void AddStorage<T>(out ComponentStorage<T> componentStorage) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (storage.TryGetValue(componentName, out var cStorage))
		{
			componentStorage = (cStorage as ComponentStorage<T>)!;
			return;
		}

		componentStorage = new();
		storage[componentName] = componentStorage;
	}


	private void AddStorage(Type componentType, out ComponentStorage componentStorage)
	{
		var method = GetType()
			.GetMethods( BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(m => m.Name == nameof(AddStorage) && m.IsGenericMethod)
			.First()
			.MakeGenericMethod(componentType);

		var arguments = new object[1];
		method.Invoke(this, arguments);

		componentStorage = (ComponentStorage) arguments[0];
	}
}