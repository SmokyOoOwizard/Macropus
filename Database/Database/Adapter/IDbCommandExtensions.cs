using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

// ReSharper disable InconsistentNaming

namespace Macropus.Database.Adapter;

public static class IDbCommandExtensions
{
    private static readonly ConcurrentDictionary<Type, CommandAdapter> CommandAdapters = new();

    public static Task<int> ExecuteNonQueryAsync(this IDbCommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (command is DbCommand dbCommand) return dbCommand.ExecuteNonQueryAsync();

        return CommandAdapters.GetOrAdd(command.GetType(), type => new CommandAdapter(type))
            .ExecuteNonQueryAsync(command);
    }

    public static Task<int> ExecuteNonQueryAsync(this IDbCommand command, CancellationToken token)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (command is DbCommand dbCommand) return dbCommand.ExecuteNonQueryAsync(token);

        return CommandAdapters.GetOrAdd(command.GetType(), type => new CommandAdapter(type))
            .ExecuteNonQueryAsyncToken(command, token);
    }

    public static Task<IDataReader> ExecuteReaderAsync(this IDbCommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (command is DbCommand dbCommand)
            return dbCommand.ExecuteReaderAsync().ContinueWith<IDataReader>(t => t.Result);

        return CommandAdapters.GetOrAdd(command.GetType(), type => new CommandAdapter(type))
            .ExecuteReaderAsync(command);
    }

    public static Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, CancellationToken token)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (command is DbCommand dbCommand)
            return dbCommand.ExecuteReaderAsync(token).ContinueWith<IDataReader>(t => t.Result, token);

        return CommandAdapters.GetOrAdd(command.GetType(), type => new CommandAdapter(type))
            .ExecuteReaderAsyncToken(command, token);
    }

    public static Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, CommandBehavior commandBehavior)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (command is DbCommand dbCommand)
            return dbCommand.ExecuteReaderAsync(commandBehavior).ContinueWith<IDataReader>(t => t.Result);

        return CommandAdapters.GetOrAdd(command.GetType(), type => new CommandAdapter(type))
            .ExecuteReaderAsyncBehavior(command, commandBehavior);
    }

    public static Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, CommandBehavior commandBehavior,
        CancellationToken token)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));


        if (command is DbCommand dbCommand)
            return dbCommand.ExecuteReaderAsync(commandBehavior, token)
                .ContinueWith<IDataReader>(t => t.Result, token);

        return CommandAdapters.GetOrAdd(command.GetType(), type => new CommandAdapter(type))
            .ExecuteReaderAsyncBehaviorToken(command, commandBehavior, token);
    }

    public static Task<object?> ExecuteScalarAsync(this IDbCommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (command is DbCommand dbCommand) return dbCommand.ExecuteScalarAsync();

        return CommandAdapters.GetOrAdd(command.GetType(), type => new CommandAdapter(type))
            .ExecuteScalarAsync(command);
    }

    public static Task<object?> ExecuteScalarAsync(this IDbCommand command, CancellationToken token)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (command is DbCommand dbCommand) return dbCommand.ExecuteScalarAsync(token);

        return CommandAdapters.GetOrAdd(command.GetType(), type => new CommandAdapter(type))
            .ExecuteScalarAsyncToken(command, token);
    }
}