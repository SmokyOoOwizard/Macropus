using Autofac;
using Macropus.Project.Instance;

namespace Macropus.Project.Connection.Impl;

internal class ProjectConnection : IProjectConnection
{
	private readonly IProjectInstance project;
	private readonly ILifetimeScope scope;

	public IProjectInformation ProjectInformation => project.ProjectInformation;

	public ProjectConnection(IProjectInstance project, ILifetimeScope scope)
	{
		this.project = project;
		this.scope = scope;
	}


	public void Dispose()
	{
		project.Dispose();
	}
}