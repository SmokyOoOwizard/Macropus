using Macropus.Project.Impl;
using Macropus.Project.Raw;
using Xunit.Abstractions;

namespace Macropus.Project.Tests;

public abstract class TestsWithProject : TestsWithProjectsStorage
{
	public virtual IProjectCreationInfo ProjectCreateInfo { get; } = new ProjectCreationInfo() { Name = "TestProject" };

	public IRawProject ProjectConnection { get; private set; } = null!;

	protected TestsWithProject(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		
		var id = await ProjectStorage.CreateProjectAsync(ProjectCreateInfo).ConfigureAwait(false);

		ProjectConnection = await ProjectStorage.OpenProjectAsync(id).ConfigureAwait(false);
	}

	public override async Task DisposeAsync()
	{
		ProjectConnection.Dispose();

		await base.DisposeAsync();
	}
}