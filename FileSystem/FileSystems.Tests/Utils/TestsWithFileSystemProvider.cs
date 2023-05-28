using Autofac;
using Macropus.Database;
using Macropus.FileSystem;
using Macropus.FileSystem.Interfaces;
using Tests.Utils;
using Xunit.Abstractions;

#pragma warning disable CS8618

namespace FileSystems.Tests.Utils;

public abstract class TestsWithFileSystemProvider : TestsWithFiles
{
	public IFileSystemService FileSystem { get; private set; }

	public TestsWithFileSystemProvider(ITestOutputHelper output) : base(output) { }

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);

		builder.RegisterModule<DatabasesContainerBuilder>();
		builder.RegisterModule<FileSystemContainerBuilder>();
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		FileSystem = await Container.Resolve<IFileSystemServiceFactory>()
			.Create(ExecutePath, Path.Combine(ExecutePath, "fs.db"));
	}

	public override async Task DisposeAsync()
	{
		await FileSystem.DisposeAsync();
		await base.DisposeAsync();
	}
}