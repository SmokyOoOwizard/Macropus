using Macropus.Database.Impl;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Database;

public class DatabasesProviderStaticTests : TestsWithFileSystemProvider
{
	public DatabasesProviderStaticTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async void CreateProviderTest()
	{
		var dbsProvider = await DatabasesProvider.Create(ExecutePath, FileSystemProvider).ConfigureAwait(false);

		Assert.NotNull(dbsProvider);

		dbsProvider.Dispose();
	}
}