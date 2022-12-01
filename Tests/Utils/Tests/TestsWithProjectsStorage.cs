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

		ProjectStorage = Mock.Create<ProjectsStorageMaster>();

		var storage = Mock.Create<ProjectsStorageLocalFactory>().Create(ExecutePath);
		ProjectStorage.AddStorage(storage);
		ProjectStorage.SetDefaultStorage(storage);
	}
}