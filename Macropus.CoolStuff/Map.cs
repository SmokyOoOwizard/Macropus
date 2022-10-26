using System.Collections;

namespace Macropus.CoolStuff;

public class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>> where T2 : notnull where T1 : notnull
{
	private readonly Dictionary<T1, T2> forward = new();
	private readonly Dictionary<T2, T1> reverse = new();

	public Map()
	{
		Forward = new Indexer<T1, T2>(forward);
		Reverse = new Indexer<T2, T1>(reverse);
	}

	public Indexer<T1, T2> Forward { get; }
	public Indexer<T2, T1> Reverse { get; }

	public void Add(T1 t1, T2 t2)
	{
		forward.Add(t1, t2);
		reverse.Add(t2, t1);
	}

	public void Remove(T1 t1)
	{
		var revKey = Forward[t1];
		forward.Remove(t1);
		reverse.Remove(revKey);
	}

	public void Remove(T2 t2)
	{
		var forwardKey = Reverse[t2];
		reverse.Remove(t2);
		forward.Remove(forwardKey);
	}

	public void Clear()
	{
		forward.Clear();
		reverse.Clear();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
	{
		return forward.GetEnumerator();
	}

	public class Indexer<T3, T4> where T3 : notnull
	{
		private readonly Dictionary<T3, T4> dictionary;

		public Indexer(Dictionary<T3, T4> dictionary)
		{
			this.dictionary = dictionary;
		}

		public T4 this[T3 index]
		{
			get => dictionary[index];
			set => dictionary[index] = value;
		}

		public bool Contains(T3 key)
		{
			return dictionary.ContainsKey(key);
		}

		public bool TryGetValue(T3 key, out T4 value)
		{
			return dictionary.TryGetValue(key, out value!);
		}
	}
}