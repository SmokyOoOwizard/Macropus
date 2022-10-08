using System.Collections;
using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage.Impl;

internal class EnumerationComponentsRefComponentsStorage : IEnumerationComponents
{
	private readonly Dictionary<string, Dictionary<Guid, IComponent?>> storage;

	public int Count => storage.Count;

	public EnumerationComponentsRefComponentsStorage(Dictionary<string, Dictionary<Guid, IComponent?>> storage)
	{
		this.storage = storage;
	}

	public IEnumerator<KeyValuePair<string, IEnumerable<Guid>>> GetEnumerator()
	{
		return storage.Select(kv =>
				new KeyValuePair<string, IEnumerable<Guid>>(kv.Key, kv.Value.Keys))
			.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}


	public bool ContainsKey(string key)
	{
		return storage.ContainsKey(key);
	}

	public bool TryGetValue(string key, out IEnumerable<Guid> value)
	{
		var result = storage.TryGetValue(key, out var entities);
		value = result ? entities!.Keys : null!;

		return result;
	}

	public IEnumerable<Guid> this[string key] => throw new NotImplementedException();

	public IEnumerable<string> Keys => storage.Keys;

	public IEnumerable<IEnumerable<Guid>> Values => storage.Values.Select(kv => kv.Keys);

	public IEnumerable<Guid> GetEntities()
	{
		return storage
			.SelectMany(j => j.Value)
			.Select(kv => kv.Key)
			.Distinct();
	}
}