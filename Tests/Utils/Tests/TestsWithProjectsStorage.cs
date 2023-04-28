using Autofac;
using Macropus.Project.Storage.Impl;
using Xunit.Abstractions;

namespace Tests.Utils.Tests;

public abstract class TestsWithProjectsStorage : TestsWithFileSystemProvider
{
	public ProjectsStorageMaster ProjectStorage { get; private set; }

	public TestsWithProjectsStorage(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		ProjectStorage = Container.Resolve<ProjectsStorageMaster>();

		var storage = Container.Resolve<ProjectsStorageLocalFactory>().Create(ExecutePath);
		ProjectStorage.AddStorage(storage);
		ProjectStorage.SetDefaultStorage(storage);
	}
}