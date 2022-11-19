using Autofac;
using Macropus.Project.Provider.Impl;

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
		return new ProjectsStorageLocal(path, scope.Resolve<ProjectProviderFactory>());
	}
}