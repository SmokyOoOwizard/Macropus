using Autofac;
using Macropus.CoolStuff;
using Macropus.Interfaces.Project;
using Macropus.Project.Raw;

namespace Macropus.Project.Instance.Impl;

internal sealed class ProjectInstance : IProjectInstance, IMakeRef<IProjectInstance>
{
	private readonly ILifetimeScope scope;
	private readonly IRawProject rawProject;

	public IProjectInformation ProjectInformation => rawProject.ProjectInformation;

	private bool disposed;

	public ProjectInstance(ILifetimeScope scope, IRawProject rawProject)
	{
		this.scope = scope;
		this.rawProject = rawProject;
	}
	
	
	

	public void Dispose()
	{
		if (disposed) return;
		disposed = true;

		scope.Dispose();
	}

	public IProjectInstance MakeRef()
	{
		// TODO
		return this;
	}
}