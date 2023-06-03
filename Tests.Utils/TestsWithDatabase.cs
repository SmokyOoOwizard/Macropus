using System.Data;
using System.Data.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using Xunit.Abstractions;

#pragma warning disable CS8618

namespace Tests.Utils;

public abstract class TestsWithDatabase : TestsWithFiles
{
	public IDbConnection DbConnection { get; private set; }
	public DataConnection DataConnection { get; private set; }
	
	public TestsWithDatabase(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		
		DbConnection = new SqliteConnection($"Data Source={Path.Combine(ExecutePath, "Test.db")}");
		DbConnection.Open();

		DataConnection = SQLiteTools.CreateDataConnection((DbConnection)DbConnection);
		
		var cmd = DbConnection.CreateCommand();
		cmd.CommandText = "PRAGMA journal_mode = WAL";
		cmd.ExecuteNonQuery();

		cmd.CommandText = "PRAGMA synchronous = NORMAL";
		cmd.ExecuteNonQuery();
		
		cmd.Dispose();
	}

	public override async Task DisposeAsync()
	{
		DbConnection.Dispose();

		await base.DisposeAsync();
	}
}