using Nito.AsyncEx;

namespace Macropus.Stuff.Cache;

internal sealed partial class CacheWithRefs<T, K> : IDisposable where K : notnull
{
    private readonly Dictionary<K, CachePart> cache = new();
    private readonly AsyncLock asyncLock = new();

    private bool disposed;


    public bool ContainsKey(K key)
    {
        using (asyncLock.Lock())
        {
            return cache.ContainsKey(key);
        }
    }

    public ICacheRef<T, K> GetOrAdd(K key, T value)
    {
        if (disposed) throw new ObjectDisposedException(nameof(CacheWithRefs<T, K>));

        using (asyncLock.Lock())
        {
            if (!cache.ContainsKey(key)) cache.Add(key, new CachePart { Cache = value, Key = key });

            var cachePart = cache[key];
            Interlocked.Increment(ref cachePart.Refs);

            return new CacheRef(cachePart, onRefDisposed);
        }
    }

    public async Task<ICacheRef<T, K>> GetOrAddAsync(K key, Task<T> func, CancellationToken cancellationToken = default)
    {
        if (disposed) throw new ObjectDisposedException(nameof(CacheWithRefs<T, K>));

        using (await asyncLock.LockAsync(cancellationToken))
        {
            if (!cache.ContainsKey(key)) cache.Add(key, new CachePart { Cache = await func, Key = key });

            var cachePart = cache[key];
            Interlocked.Increment(ref cachePart.Refs);

            return new CacheRef(cachePart, onRefDisposed);
        }
    }

    public ICacheRef<T, K> Get(K key)
    {
        if (disposed) throw new ObjectDisposedException(nameof(CacheWithRefs<T, K>));

        using (asyncLock.Lock())
        {
            if (!cache.ContainsKey(key))
                throw new KeyNotFoundException();

            var cachePart = cache[key];
            Interlocked.Increment(ref cachePart.Refs);

            return new CacheRef(cachePart, onRefDisposed);
        }
    }

    private void onRefDisposed(ICacheRef<T, K> handle)
    {
        using (asyncLock.Lock())
        {
            var cachePart = cache[handle.Key];
            Interlocked.Decrement(ref cachePart.Refs);

            if (cachePart.Refs == 0)
            {
                cache.Remove(handle.Key);

                if (cachePart.Cache is IDisposable disposable) disposable.Dispose();
            }
        }
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;

        using (asyncLock.Lock())
        {
            foreach (var cachePart in cache)
                if (cachePart.Value.Cache is IDisposable disposable)
                    disposable.Dispose();
        }

        cache.Clear();
    }
}