using System.Data;
using System.Reflection;

namespace Macropus.Db.Adapter;

internal class ConnectionAdapter
{
    internal readonly Func<IDbConnection, Task> OpenAsync;
    internal readonly Func<IDbConnection, CancellationToken, Task> OpenAsyncToken;

    internal ConnectionAdapter(Type type)
    {
        if (type.GetRuntimeMethod("OpenAsync", new Type[0]) != null)
            OpenAsync = async connection =>
            {
                dynamic cmd = connection;
                await cmd.OpenAsync();
            };
        else
            OpenAsync = async connection => await Task.Run(() => { connection.Open(); });

        if (type.GetRuntimeMethod("OpenAsync", new[] { typeof(CancellationToken) }) != null)
            OpenAsyncToken = async (connection, token) =>
            {
                dynamic cmd = connection;
                await cmd.OpenAsync(token);
            };
        else
            OpenAsyncToken = async (connection, token) => await Task.Run(() => { connection.Open(); });
    }
}