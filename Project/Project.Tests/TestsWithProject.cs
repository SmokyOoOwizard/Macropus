using Autofac;
using Macropus.Project.Impl;
using Macropus.Project.Instance;
using Xunit.Abstractions;

namespace Macropus.Project.Tests;

public abstract class TestsWithProject : TestsWithProjectsStorage
{
	public virtual IProjectCreationInfo ProjectCreateInfo { get; } = new ProjectCreationInfo() {Name = "TestProject"};

	public IProjectInstance ProjectConnection { get; private set; } = null!;
	public IProjectService ProjectService { get; private set; } = null!;

	protected TestsWithProject(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		ProjectService = Container.Resolve<IProjectService>();

		var id = await ProjectStorage.CreateProjectAsync(ProjectCreateInfo).ConfigureAwait(false);

		ProjectConnection = await ProjectService.GetOrLoadAsync(id);
	}

	public override async Task DisposeAsync()
	{
		ProjectConnection.Dispose();

		await base.DisposeAsync();
	}
}