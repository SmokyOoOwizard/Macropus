using Macropus.ECS.Component;
using Macropus.ECS.Component.Exceptions;

namespace Macropus.ECS.ComponentsStorage.Impl;

public class ComponentTypesStorage : IComponentTypesStorage
{
	private readonly Dictionary<string, uint> types = new();

	private uint idCounter;

	public uint GetComponentTypeId<T>() where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (!types.TryGetValue(componentName, out var typeId))
		{
			typeId = Interlocked.Increment(ref idCounter);
			types.Add(componentName, typeId);
		}

		return typeId;
	}

	public uint GetComponentTypeId(string name)
	{
		if (!types.TryGetValue(name, out var typeId))
		{
			typeId = Interlocked.Increment(ref idCounter);
			types.Add(name, typeId);
		}

		return typeId;
	}
}