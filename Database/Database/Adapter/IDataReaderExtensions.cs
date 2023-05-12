using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace Macropus.Database.Adapter;

public static class IDataReaderExtensions
{
    private static readonly ConcurrentDictionary<Type, DataReaderAdapter> DataReaderAdapters = new();

    public static Task<T> GetFieldValueAsync<T>(this IDataReader reader, int ordinal)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var dataReader = reader as DbDataReader;
        if (dataReader != null) return dataReader.GetFieldValueAsync<T>(ordinal);
        return DataReaderAdapters.GetOrAdd(reader.GetType(),
                type => new DataReaderAdapter(type))
            .DoGetFieldValueAsync<T>(reader, ordinal);
    }

    public static Task<T> GetFieldValueAsync<T>(this IDataReader reader, int ordinal, CancellationToken token)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var dataReader = reader as DbDataReader;
        if (dataReader != null) return dataReader.GetFieldValueAsync<T>(ordinal, token);
        return DataReaderAdapters.GetOrAdd(reader.GetType(),
                type => new DataReaderAdapter(type))
            .DoGetFieldValueAsync<T>(reader, ordinal, token);
    }

    public static Task<bool> IsDBNullAsync(this IDataReader reader, int ordinal)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var dataReader = reader as DbDataReader;
        if (dataReader != null) return dataReader.IsDBNullAsync(ordinal);
        return DataReaderAdapters.GetOrAdd(reader.GetType(),
                type => new DataReaderAdapter(type))
            .IsDBNullAsync(reader, ordinal);
    }

    public static Task<bool> IsDBNullAsync(this IDataReader reader, int ordinal, CancellationToken token)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var dataReader = reader as DbDataReader;
        if (dataReader != null) return dataReader.IsDBNullAsync(ordinal, token);
        return DataReaderAdapters.GetOrAdd(reader.GetType(),
                type => new DataReaderAdapter(type))
            .IsDBNullAsyncToken(reader, ordinal, token);
    }

    public static Task<bool> NextResultAsync(this IDataReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var dataReader = reader as DbDataReader;
        if (dataReader != null) return dataReader.NextResultAsync();
        return DataReaderAdapters.GetOrAdd(reader.GetType(),
                type => new DataReaderAdapter(type))
            .NextResultAsync(reader);
    }

    public static Task<bool> NextResultAsync(this IDataReader reader, CancellationToken token)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var dataReader = reader as DbDataReader;
        if (dataReader != null) return dataReader.NextResultAsync(token);
        return DataReaderAdapters.GetOrAdd(reader.GetType(),
                type => new DataReaderAdapter(type))
            .NextResultAsyncToken(reader, token);
    }

    public static Task<bool> ReadAsync(this IDataReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var dataReader = reader as DbDataReader;
        if (dataReader != null) return dataReader.ReadAsync();
        return DataReaderAdapters.GetOrAdd(reader.GetType(),
                type => new DataReaderAdapter(type))
            .ReadAsync(reader);
    }

    public static Task<bool> ReadAsync(this IDataReader reader, CancellationToken token)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var dataReader = reader as DbDataReader;
        if (dataReader != null) return dataReader.ReadAsync(token);
        return DataReaderAdapters.GetOrAdd(reader.GetType(),
                type => new DataReaderAdapter(type))
            .ReadAsyncToken(reader, token);
    }
}