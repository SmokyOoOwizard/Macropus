using Macropus.Project.Impl;
using Macropus.Project.Storage.Impl;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.Project.Storage;

public class LocalStorageTests : TestsWithProjectsStorage
{
	private const string PROJECT_NAME = "CreateTest";

	public LocalStorageTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async void CreateByNameTest()
	{
		var projectId = await ProjectStorage
			.CreateProjectAsync(new ProjectCreationInfo { Name = PROJECT_NAME })
			.ConfigureAwait(false);

		Assert.NotEqual(Guid.Empty, projectId);

		using var project = await ProjectStorage.OpenProjectAsync(projectId);
	}
}