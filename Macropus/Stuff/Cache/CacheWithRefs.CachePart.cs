namespace Macropus.Stuff.Cache;

internal sealed partial class CacheWithRefs<T, K> where K : notnull
{
    private class CachePart
    {
        public T Cache { get; init; }
        public K Key { get; init; }

        public int Refs;
    }
}