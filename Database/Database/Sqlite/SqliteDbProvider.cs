﻿using System.Data.Common;
using Macropus.Database.Interfaces;
using Microsoft.Data.Sqlite;

namespace Macropus.Database.Sqlite;

internal sealed class SqliteDbProvider : IDbProvider
{
	public Guid Id { get; }

	public string Name { get; }

	public uint Version { get; private set; }

	private readonly string connectionString;

	public SqliteDbProvider(string connectionString, string name, Guid id)
	{
		Id = id;
		Name = name;
		this.connectionString = connectionString;

		RefreshVersion();
	}

	public void RefreshVersion()
	{
		using var connection = CreateConnection();
		connection.Open();

		Version = DbUtils.GetVersion(connection);
	}

	public DbConnection CreateConnection()
	{
		var connection = new SqliteConnection(connectionString);
		connection.Open();

		var cmd = connection.CreateCommand();
		cmd.CommandText = "PRAGMA journal_mode = WAL";
		cmd.ExecuteNonQuery();

		cmd.CommandText = "PRAGMA synchronous = NORMAL";
		cmd.ExecuteNonQuery();
		
		cmd.Dispose();

		return connection;
	}
}