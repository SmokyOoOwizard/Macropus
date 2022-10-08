using System.Collections;
using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage.Impl;

internal class EnumerationComponentsCopyComponentsStorage : IEnumerationComponents
{
	private readonly Dictionary<string, IEnumerable<Guid>> components;
	private readonly ISet<Guid> entities;

	public int Count => components.Count;
	public IEnumerable<string> Keys => components.Keys;
	public IEnumerable<IEnumerable<Guid>> Values => components.Values;

	public EnumerationComponentsCopyComponentsStorage(Dictionary<string, Dictionary<Guid, IComponent?>> entities)
	{
		components = new Dictionary<string, IEnumerable<Guid>>(entities.Count);
		this.entities = new HashSet<Guid>();
		foreach (var component in entities)
		{
			components[component.Key] = component.Value.Keys.ToHashSet();

			this.entities.UnionWith(component.Value.Keys);
		}
	}


	public IEnumerator<KeyValuePair<string, IEnumerable<Guid>>> GetEnumerator()
	{
		return components.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool ContainsKey(string key)
	{
		return components.ContainsKey(key);
	}

	public bool TryGetValue(string key, out IEnumerable<Guid> value)
	{
		return components.TryGetValue(key, out value!);
	}

	public IEnumerable<Guid> this[string key] => components[key];

	public IEnumerable<Guid> GetEntities()
	{
		return entities;
	}
}