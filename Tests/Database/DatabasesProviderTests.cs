using Macropus.Database;
using Macropus.Database.Adapter;
using Tests.Utils.Random;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Database;

public class DatabasesProviderTests : TestsWithDatabasesProvider
{
	public DatabasesProviderTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async void CreateDatabaseTest()
	{
		var dbName = RandomUtils.GetRandomString(64);

		Assert.True(await DatabasesService.CreateDatabaseAsync(dbName).ConfigureAwait(false));

		var db = await DatabasesService.TryGetDatabase(dbName).ConfigureAwait(false);

		Assert.NotNull(db);
	}

	[Fact]
	public async void DeleteDatabaseTest()
	{
		var dbName = RandomUtils.GetRandomString(64);

		Assert.True(await DatabasesService.CreateDatabaseAsync(dbName).ConfigureAwait(false));

		Assert.True(await DatabasesService.DeleteDatabaseAsync(dbName).ConfigureAwait(false));

		var db = await DatabasesService.TryGetDatabase(dbName).ConfigureAwait(false);

		if (db != null)
			Assert.Fail("Database must not exist after delete");
	}

	[Fact]
	public async void ReadWriteDatabaseTest()
	{
		var dbName = RandomUtils.GetRandomString(64);

		Assert.True(await DatabasesService.CreateDatabaseAsync(dbName).ConfigureAwait(false));

		using var db = await DatabasesService.TryGetDatabase(dbName).ConfigureAwait(false);

		Assert.NotNull(db);

		await db.OpenAsync().ConfigureAwait(false);

		DbUtils.GetTableCount(db);

		Assert.Equal(0u, DbUtils.GetVersion(db));

		var dbVersion = RandomUtils.GetRandomByte();

		DbUtils.SetVersion(db, dbVersion);

		Assert.Equal(dbVersion, DbUtils.GetVersion(db));
	}
}