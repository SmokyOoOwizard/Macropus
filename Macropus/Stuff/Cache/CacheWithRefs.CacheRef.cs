namespace Macropus.Stuff.Cache;

internal sealed partial class CacheWithRefs<T, K> where K : notnull
{
    private class CacheRef : ICacheRef<T, K>
    {
        private readonly CachePart part;
        private readonly Action<CacheRef> onDisposed;

        public T Value => part.Cache;
        public K Key => part.Key;

        private bool disposed;

        public CacheRef(CachePart part, Action<CacheRef> onDisposed)
        {
            this.part = part;
            this.onDisposed = onDisposed;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            onDisposed?.Invoke(this);
        }
    }
}