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
	}

	public override async Task DisposeAsync()
	{
		DbConnection.TryDispose();

		await base.DisposeAsync();
	}
}