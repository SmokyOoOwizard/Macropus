using Autofac;
using Macropus.Project.Impl;
using Macropus.Project.Instance;
using Xunit.Abstractions;

namespace Macropus.Project.Tests.Storage;

public class LocalStorageTests : TestsWithProjectsStorage
{
	private const string PROJECT_NAME = "CreateTest";

	public LocalStorageTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async void CreateByName()
	{
		var projectId = await ProjectStorage
			.CreateProjectAsync(new ProjectCreationInfo {Name = PROJECT_NAME});

		Assert.NotEqual(Guid.Empty, projectId);

		await using var project = await ProjectStorage.OpenProjectAsync(projectId);

		Assert.Equal(PROJECT_NAME, project.ProjectInformation.Name);
	}

	[Fact]
	public async void LoadInstance()
	{
		var projectService = Container.Resolve<IProjectService>();

		var projectId = await ProjectStorage
			.CreateProjectAsync(new ProjectCreationInfo {Name = PROJECT_NAME});

		using var projectConnection = await projectService.GetOrLoadAsync(projectId);

		Assert.NotNull(projectConnection);
	}
}