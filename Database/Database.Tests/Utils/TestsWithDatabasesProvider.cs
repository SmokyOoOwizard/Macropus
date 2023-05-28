using Autofac;
using Macropus.Database.Interfaces;
using Xunit.Abstractions;

#pragma warning disable CS8618

namespace Database.Tests.Utils;

public abstract class TestsWithDatabasesProvider : TestsWithFileSystemProvider
{
	public IDatabasesService DatabasesService { get; private set; }

	public TestsWithDatabasesProvider(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		DatabasesService = await Container.Resolve<IDatabasesServiceFactory>()
			.Create(Path.Combine(ExecutePath, "db"), FileSystem);
	}

	public override async Task DisposeAsync()
	{
		DatabasesService.Dispose();

		await base.DisposeAsync();
	}
}