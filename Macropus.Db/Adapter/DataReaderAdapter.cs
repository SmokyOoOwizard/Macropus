using System.Data;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace Macropus.Db.Adapter;

internal class DataReaderAdapter
{
    internal readonly Func<IDataReader, int, Task<bool>> IsDBNullAsync;
    internal readonly Func<IDataReader, int, CancellationToken, Task<bool>> IsDBNullAsyncToken;
    internal readonly Func<IDataReader, Task<bool>> NextResultAsync;
    internal readonly Func<IDataReader, CancellationToken, Task<bool>> NextResultAsyncToken;
    internal readonly Func<IDataReader, Task<bool>> ReadAsync;
    internal readonly Func<IDataReader, CancellationToken, Task<bool>> ReadAsyncToken;

    private readonly MethodInfo? getFieldValueAsync;
    private readonly MethodInfo? getFieldValueAsyncToken;

    internal DataReaderAdapter(Type type)
    {
        if (type.GetRuntimeMethod("IsDBNullAsync", new[] { typeof(int) }) != null)
            IsDBNullAsync = async (reader, ordinal) =>
            {
                dynamic cmd = reader;
                return await cmd.IsDBNullAsync(ordinal);
            };
        else
            IsDBNullAsync = async (reader, ordinal) => await Task.FromResult(reader.IsDBNull(ordinal));

        if (type.GetRuntimeMethod("IsDBNullAsync", new[] { typeof(int), typeof(CancellationToken) }) != null)
            IsDBNullAsyncToken = async (reader, ordinal, token) =>
            {
                dynamic cmd = reader;
                return await cmd.IsDBNullAsync(ordinal, token);
            };
        else
            IsDBNullAsyncToken = async (reader, ordinal, _) => await Task.FromResult(reader.IsDBNull(ordinal));

        if (type.GetRuntimeMethod("NextResultAsync", Type.EmptyTypes) != null)
            NextResultAsync = async reader =>
            {
                dynamic cmd = reader;
                return await cmd.NextResultAsync();
            };
        else
            NextResultAsync = async reader => await Task.FromResult(reader.NextResult());

        if (type.GetRuntimeMethod("NextResultAsync", new[] { typeof(int), typeof(CancellationToken) }) != null)
            NextResultAsyncToken = async (reader, token) =>
            {
                dynamic cmd = reader;
                return await cmd.NextResultAsync(token);
            };
        else
            NextResultAsyncToken = async (reader, _) => await Task.FromResult(reader.NextResult());

        if (type.GetRuntimeMethod("ReadAsync", Type.EmptyTypes) != null)
            ReadAsync = async reader =>
            {
                dynamic cmd = reader;
                return await cmd.ReadAsync();
            };
        else
            ReadAsync = async reader => await Task.FromResult(reader.Read());

        if (type.GetRuntimeMethod("ReadAsync", new[] { typeof(int), typeof(CancellationToken) }) != null)
            ReadAsyncToken = async (reader, token) =>
            {
                dynamic cmd = reader;
                return await cmd.ReadAsync(token);
            };
        else
            ReadAsyncToken = async (reader, _) => await Task.FromResult(reader.Read());

        // for template function we have to defer checks
        getFieldValueAsync = type.GetRuntimeMethod("GetFieldValueAsync", new[] { typeof(int) });
        getFieldValueAsyncToken =
            type.GetRuntimeMethod("GetFieldValueAsync", new[] { typeof(int), typeof(CancellationToken) });
    }


    internal Task<T> DoGetFieldValueAsync<T>(IDataReader reader, int ordinal)
    {
        if (getFieldValueAsync != null)
        {
            var method = getFieldValueAsync.MakeGenericMethod(typeof(T));
            return (Task<T>)method.Invoke(reader, new object[] { ordinal })!;
        }

        return Task.FromResult((T)reader.GetValue(ordinal));
    }

    internal Task<T> DoGetFieldValueAsync<T>(IDataReader reader, int ordinal, CancellationToken token)
    {
        if (getFieldValueAsyncToken != null)
        {
            var method = getFieldValueAsyncToken.MakeGenericMethod(typeof(T));
            return (Task<T>)method.Invoke(reader, new object[] { ordinal, token })!;
        }

        return Task.FromResult((T)reader.GetValue(ordinal));
    }
}