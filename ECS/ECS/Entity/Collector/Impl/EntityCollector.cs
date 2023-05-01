using System;
using System.Collections.Generic;
using Macropus.ECS.Component.Trigger;

namespace Macropus.ECS.Entity.Collector.Impl;

public class EntityCollector : IEntityCollector
{
	private readonly List<Guid> entities = new();

	public ComponentsTrigger Trigger { get; }

	public int Count => entities.Count;


	public EntityCollector(ComponentsTrigger trigger)
	{
		Trigger = trigger;
	}

	public IEnumerable<Guid> GetEntities()
	{
		return entities;
	}

	public void AddEntity(Guid entity)
	{
		entities.Add(entity);
	}

	public void Clear()
	{
		entities.Clear();
	}
}