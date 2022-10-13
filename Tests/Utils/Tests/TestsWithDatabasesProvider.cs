using Macropus.Database;
using Xunit.Abstractions;

#pragma warning disable CS8618

namespace Tests.Utils.Tests;

public abstract class TestsWithDatabasesProvider : TestsWithFileSystemProvider
{
	public IDatabasesProvider DatabasesProvider { get; private set; }

	public TestsWithDatabasesProvider(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync().ConfigureAwait(false);

		DatabasesProvider = await Macropus.Database.Impl.DatabasesProvider.Create(ExecutePath, FileSystemProvider)
			.ConfigureAwait(false);
	}

	public override async Task DisposeAsync()
	{
		DatabasesProvider.Dispose();

		await base.DisposeAsync();
	}
}