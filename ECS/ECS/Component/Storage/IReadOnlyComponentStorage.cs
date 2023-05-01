using System;
using System.Collections.Generic;
using Macropus.ECS.Component.Storage.Impl;

namespace Macropus.ECS.Component.Storage;

public interface IReadOnlyComponentStorage : IEnumerable<KeyValuePair<Guid, IComponent?>>
{
	string ComponentName { get; }
	bool HasEntity(Guid entity);
	IComponent? GetComponent(Guid entity);
	IReadOnlyCollection<Guid> GetEntities();
	ComponentStorage DeepClone();
}