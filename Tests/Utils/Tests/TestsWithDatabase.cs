using System.Data;
using Macropus.Database.Adapter;
using Macropus.Extensions;
using Microsoft.Data.Sqlite;
using Xunit.Abstractions;

#pragma warning disable CS8618

namespace Tests.Utils.Tests;

public abstract class TestsWithDatabase : TestsWithFiles, IAsyncLifetime
{
	public IDbConnection DbConnection { get; private set; }
	public TestsWithDatabase(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		
		DbConnection = new SqliteConnection($"Data Source={Path.Combine(ExecutePath, "Test.db")}");
		await DbConnection.OpenAsync();
		
		var cmd = DbConnection.CreateCommand();
		cmd.CommandText = "PRAGMA journal_mode = WAL";
		cmd.ExecuteNonQuery();

		cmd.CommandText = "PRAGMA synchronous = NORMAL";
		cmd.ExecuteNonQuery();
		
		cmd.Dispose();
	}

	public override async Task DisposeAsync()
	{
		DbConnection.TryDispose();

		await base.DisposeAsync();
	}
}