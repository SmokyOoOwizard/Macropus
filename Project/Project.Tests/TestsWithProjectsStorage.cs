using Autofac;
using Macropus.Database;
using Macropus.FileSystem;
using Macropus.FileSystem.Interfaces;
using Macropus.Project.Storage.Impl;
using Tests.Utils;
using Xunit.Abstractions;

namespace Macropus.Project.Tests;

public abstract class TestsWithProjectsStorage : TestsWithFiles
{
	public ProjectsStorageMaster ProjectStorage { get; private set; }
	public IFileSystemService FileSystem { get; private set; }

	public TestsWithProjectsStorage(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		FileSystem = await Container.Resolve<IFileSystemServiceFactory>()
			.Create(ExecutePath, Path.Combine(ExecutePath, "fs.db"));

		ProjectStorage = Container.Resolve<ProjectsStorageMaster>();

		var storage = Container.Resolve<ProjectsStorageLocalFactory>().Create(ExecutePath);
		ProjectStorage.AddStorage(storage);
		ProjectStorage.SetDefaultStorage(storage);
	}

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);

		builder.RegisterModule<DatabasesContainerBuilder>();
		builder.RegisterModule<FileSystemContainerBuilder>();
		builder.RegisterModule<ProjectContainerBuilder>();
	}

	public override async Task DisposeAsync()
	{
		await FileSystem.DisposeAsync();
		await base.DisposeAsync();
	}
}