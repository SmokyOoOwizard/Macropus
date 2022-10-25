using System.Collections;

namespace Macropus.CoolStuff;

public class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>> where T2 : notnull where T1 : notnull
{
	private readonly Dictionary<T1, T2> _forward = new();
	private readonly Dictionary<T2, T1> _reverse = new();

	public Map()
	{
		Forward = new Indexer<T1, T2>(_forward);
		Reverse = new Indexer<T2, T1>(_reverse);
	}

	public Indexer<T1, T2> Forward { get; }
	public Indexer<T2, T1> Reverse { get; }

	public void Add(T1 t1, T2 t2)
	{
		_forward.Add(t1, t2);
		_reverse.Add(t2, t1);
	}

	public void Remove(T1 t1)
	{
		var revKey = Forward[t1];
		_forward.Remove(t1);
		_reverse.Remove(revKey);
	}

	public void Remove(T2 t2)
	{
		var forwardKey = Reverse[t2];
		_reverse.Remove(t2);
		_forward.Remove(forwardKey);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
	{
		return _forward.GetEnumerator();
	}

	public class Indexer<T3, T4> where T3 : notnull
	{
		private readonly Dictionary<T3, T4> _dictionary;

		public Indexer(Dictionary<T3, T4> dictionary)
		{
			_dictionary = dictionary;
		}

		public T4 this[T3 index]
		{
			get => _dictionary[index];
			set => _dictionary[index] = value;
		}

		public bool Contains(T3 key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool TryGetValue(T3 key, out T4 value)
		{
			return _dictionary.TryGetValue(key, out value!);
		}
	}
}