namespace Macropus.Stuff.Cache;

internal interface ICacheRef<T> : IDisposable
{
    T Value { get; }
}

internal interface ICacheRef<T, K> : ICacheRef<T>
{
    K Key { get; }
}