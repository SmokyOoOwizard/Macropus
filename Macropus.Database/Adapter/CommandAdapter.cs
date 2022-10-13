using System.Data;
using System.Reflection;

namespace Macropus.Database.Adapter;

internal class CommandAdapter
{
    internal readonly Func<IDbCommand, Task<int>> ExecuteNonQueryAsync;
    internal readonly Func<IDbCommand, CancellationToken, Task<int>> ExecuteNonQueryAsyncToken;

    internal readonly Func<IDbCommand, Task<IDataReader>> ExecuteReaderAsync;
    internal readonly Func<IDbCommand, CancellationToken, Task<IDataReader>> ExecuteReaderAsyncToken;
    internal readonly Func<IDbCommand, CommandBehavior, Task<IDataReader>> ExecuteReaderAsyncBehavior;

    internal readonly Func<IDbCommand, CommandBehavior, CancellationToken, Task<IDataReader>>
        ExecuteReaderAsyncBehaviorToken;

    internal readonly Func<IDbCommand, Task<object?>> ExecuteScalarAsync;
    internal readonly Func<IDbCommand, CancellationToken, Task<object?>> ExecuteScalarAsyncToken;

    internal CommandAdapter(Type type)
    {
        #region ExecuteNonQueryAsync

        if (type.GetRuntimeMethod("ExecuteNonQueryAsync", new Type[0]) != null)
            ExecuteNonQueryAsync = async command =>
            {
                dynamic cmd = command;
                return await cmd.ExecuteNonQueryAsync();
            };
        else
            ExecuteNonQueryAsync = async command => await Task.FromResult(command.ExecuteNonQuery());

        if (type.GetRuntimeMethod("ExecuteNonQueryAsync", new[] { typeof(CancellationToken) }) != null)
            ExecuteNonQueryAsyncToken = async (command, token) =>
            {
                dynamic cmd = command;
                return await cmd.ExecuteNonQueryAsync(token);
            };
        else
            ExecuteNonQueryAsyncToken = async (command, token) => await Task.FromResult(command.ExecuteNonQuery());

        #endregion ExecuteNonQueryAsync

        #region ExecuteReaderAsync

        if (type.GetRuntimeMethod("ExecuteReaderAsync", new Type[0]) != null)
            ExecuteReaderAsync = async command =>
            {
                dynamic cmd = command;
                return await cmd.ExecuteReaderAsync();
            };
        else
            ExecuteReaderAsync = async command => await Task.FromResult(command.ExecuteReader());

        if (type.GetRuntimeMethod("ExecuteReaderAsync", new[] { typeof(CancellationToken) }) != null)
            ExecuteReaderAsyncToken = async (command, token) =>
            {
                dynamic cmd = command;
                return await cmd.ExecuteReaderAsync(token);
            };
        else
            ExecuteReaderAsyncToken = async (command, token) => await Task.FromResult(command.ExecuteReader());

        if (type.GetRuntimeMethod("ExecuteReaderAsync", new[] { typeof(CommandBehavior) }) != null)
            ExecuteReaderAsyncBehavior = async (command, behavior) =>
            {
                dynamic cmd = command;
                return await cmd.ExecuteReaderAsync(behavior);
            };
        else
            ExecuteReaderAsyncBehavior = async (command, behavior) => await Task.FromResult(command.ExecuteReader());

        if (type.GetRuntimeMethod("ExecuteReaderAsync", new[] { typeof(CommandBehavior), typeof(CancellationToken) }) !=
            null)
            ExecuteReaderAsyncBehaviorToken = async (command, behavior, token) =>
            {
                dynamic cmd = command;
                return await cmd.ExecuteReaderAsync(behavior, token);
            };
        else
            ExecuteReaderAsyncBehaviorToken = async (command, behavior, token) =>
                await Task.FromResult(command.ExecuteReader());

        #endregion ExecuteReaderAsync

        #region ExecuteScalarAsync

        if (type.GetRuntimeMethod("ExecuteScalarAsync", new Type[0]) != null)
            ExecuteScalarAsync = async command =>
            {
                dynamic cmd = command;
                return await cmd.ExecuteScalarAsync();
            };
        else
            ExecuteScalarAsync = async command => await Task.FromResult(command.ExecuteScalarAsync());

        if (type.GetRuntimeMethod("ExecuteScalarAsync", new[] { typeof(CancellationToken) }) != null)
            ExecuteScalarAsyncToken = async (command, token) =>
            {
                dynamic cmd = command;
                return await cmd.ExecuteScalarAsync();
            };
        else
            ExecuteScalarAsyncToken = async (command, token) => await Task.FromResult(command.ExecuteScalarAsync());

        #endregion ExecuteScalarAsync
    }
}