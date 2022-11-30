using Autofac;

namespace Macropus.Project.Storage.Impl;

public class ProjectsStorageLocalFactory
{
	private readonly IComponentContext scope;

	public ProjectsStorageLocalFactory(IComponentContext scope)
	{
		this.scope = scope;
	}

	public ProjectsStorageLocal Create(string path)
	{
		return scope.Resolve<ProjectsStorageLocal>(new NamedParameter("path", path));
	}
}