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

	public async Task InitializeAsync()
	{
		DbConnection = new SqliteConnection($"Data Source={Path.Combine(ExecutePath, "Test.db")}");
		await DbConnection.OpenAsync();
	}

	public Task DisposeAsync()
	{
		DbConnection.TryDispose();

		return Task.CompletedTask;
	}
}